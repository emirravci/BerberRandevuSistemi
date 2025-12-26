using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Infrastructure.VeriErisim;
using BerberRandevu.Web.Models.Randevu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Kullanıcı randevu oluşturma ve kendi randevularını görüntüleme işlemlerini yönetir.
/// </summary>
[Authorize(Roles = "Kullanıcı")]
public class RandevuController : Controller
{
    private readonly IRandevuServisi _randevuServisi;
    private readonly IPersonelServisi _personelServisi;
    private readonly IHizmetServisi _hizmetServisi;
    private readonly ISalonAyarlariServisi _salonAyarlariServisi;
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly BerberDbContext _dbContext;

    public RandevuController(
        IRandevuServisi randevuServisi,
        IPersonelServisi personelServisi,
        IHizmetServisi hizmetServisi,
        ISalonAyarlariServisi salonAyarlariServisi,
        UserManager<UygulamaKullanicisi> userManager,
        BerberDbContext dbContext)
    {
        _randevuServisi = randevuServisi;
        _personelServisi = personelServisi;
        _hizmetServisi = hizmetServisi;
        _salonAyarlariServisi = salonAyarlariServisi;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Olustur()
    {
        var vm = new RandevuOlusturViewModel
        {
            Personeller = (await _personelServisi.AktifPersonelleriGetirAsync()).ToList(),
            Hizmetler = (await _hizmetServisi.AktifHizmetleriGetirAsync())
     .OrderBy(h => h.Ad)
           .ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Olustur(RandevuOlusturViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Personeller = (await _personelServisi.AktifPersonelleriGetirAsync()).ToList();
            model.Hizmetler = (await _hizmetServisi.AktifHizmetleriGetirAsync())
  .OrderBy(h => h.Ad)
    .ToList();
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;

        var dto = new RandevuDto
        {
            MusteriId = userId,
            PersonelId = model.PersonelId,
            Tarih = model.Tarih.Date,
            Saat = model.Saat,
            Ucret = model.Ucret
        };

        try
        {
            await _randevuServisi.RandevuOlusturAsync(dto);
            TempData["Basari"] = "Randevunuz oluşturuldu ve onay bekliyor.";
            return RedirectToAction(nameof(BenimRandevularim));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.Personeller = (await _personelServisi.AktifPersonelleriGetirAsync()).ToList();
            model.Hizmetler = (await _hizmetServisi.AktifHizmetleriGetirAsync())
                .OrderBy(h => h.Ad)
    .ToList();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> BenimRandevularim()
    {
        var userId = _userManager.GetUserId(User)!;
        var randevular = await _randevuServisi.KullaniciRandevulariniGetirAsync(userId);
        return View(randevular);
    }

    /// <summary>
    /// Belirli bir tarih için uygun saat slotlarını döndürür
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> UygunSaatleriGetir(string tarih, int personelId, int hizmetId)
    {
        try
    {
       // Tarihi açıkça parse et - timezone sorununu önle
            if (!DateTime.TryParseExact(tarih, "yyyy-MM-dd", 
                System.Globalization.CultureInfo.InvariantCulture, 
          System.Globalization.DateTimeStyles.None, 
        out DateTime parsedTarih))
         {
        return Json(new { success = false, message = "Geçersiz tarih formatı" });
       }

     // Tarihi normalize et (sadece tarih kısmı)
parsedTarih = parsedTarih.Date;

     // DEBUG: Gelen tarihi logla
       Console.WriteLine($"DEBUG - Gelen tarih string: '{tarih}'");
 Console.WriteLine($"DEBUG - Parse edilen tarih: {parsedTarih:yyyy-MM-dd dddd}");
Console.WriteLine($"DEBUG - DayOfWeek: {parsedTarih.DayOfWeek} ({(int)parsedTarih.DayOfWeek})");

   // Salon çalışma saatlerini al
var ayarlar = await _salonAyarlariServisi.CalismaSaatleriAyarlariniGetirAsync();
            var hizmet = await _hizmetServisi.HizmetGetirAsync(hizmetId);

     if (hizmet == null)
          return Json(new { success = false, message = "Hizmet bulunamadı" });

       // DEBUG: Tüm günlük ayarları logla
       Console.WriteLine($"DEBUG - Günlük Ayarlar ({ayarlar.GunlukSaatler.Count} gün):");
            foreach (var g in ayarlar.GunlukSaatler)
     {
            Console.WriteLine($"  {g.Gun} ({(int)g.Gun}): AçıkMı={g.AcikMi}, Başlangıç={g.BaslangicSaati}, Bitiş={g.BitisSaati}");
       }

          // Seçilen gün için çalışma saatlerini kontrol et
            var gunAyari = ayarlar.GunlukSaatler.FirstOrDefault(g => g.Gun == parsedTarih.DayOfWeek);

    Console.WriteLine($"DEBUG - Aranan gün: {parsedTarih.DayOfWeek} ({(int)parsedTarih.DayOfWeek})");
        Console.WriteLine($"DEBUG - Bulunan ayar: {(gunAyari != null ? $"{gunAyari.Gun}, AçıkMı={gunAyari.AcikMi}" : "NULL")}");

   // Gün kapalıysa
  if (gunAyari == null || !gunAyari.AcikMi)
            {
  var mesaj = $"{parsedTarih:dd MMMM dddd} günü salon kapalıdır.";
 Console.WriteLine($"DEBUG - Gün kapalı mesajı: {mesaj}");
   return Json(new { success = false, message = mesaj });
            }

 // O günün çalışma saatlerini kullan
            var baslangic = gunAyari.BaslangicSaati;
            var bitis = gunAyari.BitisSaati;

            Console.WriteLine($"DEBUG - Kullanılacak saatler: {baslangic} - {bitis}");

// O günkü mevcut randevuları al
      var mevcutRandevular = await _randevuServisi.PersonelRandevulariniGetirAsync(personelId, parsedTarih);
            var mevcutRandevularFiltre = mevcutRandevular
      .Where(r => r.Tarih.Date == parsedTarih.Date && r.Durum != Domain.Enumlar.RandevuDurumu.IptalEdildi)
                .ToList();

          Console.WriteLine($"DEBUG - Mevcut randevu sayısı: {mevcutRandevularFiltre.Count}");

          // Saat slotlarını oluştur
            var slotlar = new List<object>();
            var dilim = ayarlar.RandevuDilimiDakika;
  var hizmetSuresi = hizmet.Sure;

var simdikiSaat = baslangic;
          while (simdikiSaat < bitis)
     {
  var slotBitis = simdikiSaat.Add(TimeSpan.FromMinutes(hizmetSuresi));

    // Slot hizmet süresinden fazla ise son çalışma saatini aşıyor mu kontrol et
          if (slotBitis > bitis)
      break;

         // Bu slot dolu mu?
      var doluMu = mevcutRandevularFiltre.Any(r =>
   {
     var randevuBitis = r.Saat.Add(TimeSpan.FromMinutes(hizmetSuresi));
   return (simdikiSaat >= r.Saat && simdikiSaat < randevuBitis) ||
          (slotBitis > r.Saat && slotBitis <= randevuBitis) ||
         (simdikiSaat <= r.Saat && slotBitis >= randevuBitis);
       });

   // Geçmiş saat mi?
                var simdi = DateTime.Now;
        var slotTarihSaat = parsedTarih.Date.Add(simdikiSaat);
     var gecmisMi = slotTarihSaat < simdi;

        slotlar.Add(new
  {
               saat = simdikiSaat.ToString(@"hh\:mm"),
         uygun = !doluMu && !gecmisMi,
       dolu = doluMu,
    gecmis = gecmisMi
     });

       simdikiSaat = simdikiSaat.Add(TimeSpan.FromMinutes(dilim));
   }

         Console.WriteLine($"DEBUG - Oluşturulan slot sayısı: {slotlar.Count}");
      return Json(new { success = true, slotlar });
        }
        catch (Exception ex)
        {
  Console.WriteLine($"DEBUG - HATA: {ex.Message}");
       Console.WriteLine($"DEBUG - Stack Trace: {ex.StackTrace}");
            return Json(new { success = false, message = $"Hata: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> PersonelTakvimVeri(int personelId, DateTime start, DateTime end)
    {
        // Personelin randevuları
        var randevular = await _randevuServisi.PersonelRandevulariniGetirAsync(personelId);
        var filtered = randevular
            .Where(r => r.Tarih.Date >= start.Date && r.Tarih.Date <= end.Date)
            .ToList();

        var randevuEvents = filtered.Select(r => new
        {
            id = r.Id,
            title = $"{r.Ucret:C0} - {r.Durum}",
            start = r.Tarih.Date.Add(r.Saat),
            end = r.Tarih.Date.Add(r.Saat).Add(TimeSpan.FromMinutes(45)),
            extendedProps = new
            {
                durum = r.Durum.ToString(),
                personel = r.PersonelAdSoyad,
                musteriId = r.MusteriId
            }
        });

        // Personelin çalışma saatlerini arka plan olayı olarak getir
        var calismaSaatleri = await _dbContext.CalismaSaatleri
            .Where(c => c.PersonelId == personelId)
            .ToListAsync();

        var bugun = DateTime.Today;
        var haftaBaslangic = bugun.AddDays(-(int)bugun.DayOfWeek);

        var calismaEvents = calismaSaatleri.Select(cs =>
        {
            var gunTarihi = haftaBaslangic.AddDays((int)cs.Gun);
            var bas = gunTarihi.Date.Add(cs.BaslangicSaati);
            var bit = gunTarihi.Date.Add(cs.BitisSaati);

            return new
            {
                title = "Çalışma Saati",
                start = bas,
                end = bit,
                display = "background",
                backgroundColor = "#1d4ed8",
                borderColor = "#1d4ed8"
            };
        });

        var result = new List<object>();
        result.AddRange(randevuEvents);
        result.AddRange(calismaEvents);

        return Json(result);
    }
}


