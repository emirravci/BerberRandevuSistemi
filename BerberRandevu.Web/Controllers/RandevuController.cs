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
            Hizmetler = (await _hizmetServisi.AktifHizmetleriGetirAsync()).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Olustur(RandevuOlusturViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Personeller = (await _personelServisi.AktifPersonelleriGetirAsync()).ToList();
            model.Hizmetler = (await _hizmetServisi.AktifHizmetleriGetirAsync()).ToList();
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
            model.Hizmetler = (await _hizmetServisi.AktifHizmetleriGetirAsync()).ToList();
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
    public async Task<IActionResult> UygunSaatleriGetir(int personelId, DateTime tarih, int hizmetId)
    {
        try
        {
            // Salon çalışma saatlerini al
            var ayarlar = await _salonAyarlariServisi.CalismaSaatleriAyarlariniGetirAsync();
            var hizmet = await _hizmetServisi.HizmetGetirAsync(hizmetId);

            if (hizmet == null)
                return Json(new { success = false, message = "Hizmet bulunamadı" });

            // O günkü mevcut randevuları al
            var mevcutRandevular = await _randevuServisi.PersonelRandevulariniGetirAsync(personelId, tarih);
            var mevcutRandevularFiltre = mevcutRandevular
                .Where(r => r.Tarih.Date == tarih.Date && r.Durum != Domain.Enumlar.RandevuDurumu.IptalEdildi)
                .ToList();

            // Saat slotlarını oluştur
            var slotlar = new List<object>();
            var baslangic = ayarlar.BaslangicSaati;
            var bitis = ayarlar.BitisSaati;
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
                var slotTarihSaat = tarih.Date.Add(simdikiSaat);
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

            return Json(new { success = true, slotlar });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
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


