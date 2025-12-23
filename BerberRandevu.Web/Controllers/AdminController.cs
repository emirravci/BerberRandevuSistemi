using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Yönetici paneli ve gelir-gider raporlarını yönetir.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IOdemeServisi _odemeServisi;
    private readonly IGiderServisi _giderServisi;
    private readonly IRandevuServisi _randevuServisi;
    private readonly IPersonelServisi _personelServisi;
    private readonly IHizmetServisi _hizmetServisi;
    private readonly ISalonAyarlariServisi _salonAyarlariServisi;
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
      IOdemeServisi odemeServisi,
        IGiderServisi giderServisi,
        IRandevuServisi randevuServisi,
        IPersonelServisi personelServisi,
        IHizmetServisi hizmetServisi,
        ISalonAyarlariServisi salonAyarlariServisi,
        UserManager<UygulamaKullanicisi> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _odemeServisi = odemeServisi;
        _giderServisi = giderServisi;
        _randevuServisi = randevuServisi;
_personelServisi = personelServisi;
        _hizmetServisi = hizmetServisi;
   _salonAyarlariServisi = salonAyarlariServisi;
        _userManager = userManager;
 _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var bugun = DateTime.Today;
        var ayBaslangic = new DateTime(bugun.Year, bugun.Month, 1);
        var ayBitis = ayBaslangic.AddMonths(1).AddDays(-1);

        var bugunOdemeler = await _odemeServisi.TarihAraligindaOdemeleriGetirAsync(bugun, bugun);
        var bugunGiderler = await _giderServisi.TarihAraligindaGiderleriGetirAsync(bugun, bugun);

        var ayOdemeler = await _odemeServisi.TarihAraligindaOdemeleriGetirAsync(ayBaslangic, ayBitis);
        var ayGiderler = await _giderServisi.TarihAraligindaGiderleriGetirAsync(ayBaslangic, ayBitis);

        var vm = new AdminDashboardViewModel
        {
            BugunGelir = bugunOdemeler.Sum(o => o.Tutar),
            BugunGider = bugunGiderler.Sum(g => g.Tutar),
            AyGelir = ayOdemeler.Sum(o => o.Tutar),
            AyGider = ayGiderler.Sum(g => g.Tutar)
        };

        vm.BugunNet = vm.BugunGelir - vm.BugunGider;
        vm.AyNet = vm.AyGelir - vm.AyGider;

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Kullanicilar()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var items = new List<KullaniciListeItemViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            items.Add(new KullaniciListeItemViewModel
            {
                Id = user.Id,
                Eposta = user.Email,
                AdSoyad = user.AdSoyad,
                Roller = string.Join(", ", roles),
                AktifMi = user.AktifMi
            });
        }

        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> KullaniciAktiflikDegistir(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        user.AktifMi = !user.AktifMi;

        // İsteğe bağlı: pasif kullanıcıları kilitle
        if (!user.AktifMi)
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }
        else
        {
            user.LockoutEnd = null;
        }

        await _userManager.UpdateAsync(user);
        TempData["Basari"] = "Kullanıcı güncellendi.";
        return RedirectToAction(nameof(Kullanicilar));
    }

    [HttpPost]
    public async Task<IActionResult> KullaniciRolGuncelle(KullaniciRolGuncelleViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return NotFound();

        if (!await _roleManager.RoleExistsAsync(model.Rol))
        {
            ModelState.AddModelError(string.Empty, "Geçersiz rol.");
            return RedirectToAction(nameof(Kullanicilar));
        }

        var mevcutRoller = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, mevcutRoller);
        await _userManager.AddToRoleAsync(user, model.Rol);

        TempData["Basari"] = "Kullanıcı rolü güncellendi.";
        return RedirectToAction(nameof(Kullanicilar));
    }

    [HttpGet]
    public async Task<IActionResult> Randevular()
    {
        var tumRandevular = await _randevuServisi.TumRandevulariniGetirAsync();
        return View(tumRandevular.OrderByDescending(r => r.Tarih).ThenByDescending(r => r.Saat).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> RandevuOnayla(int id, string? returnUrl = null)
    {
        await _randevuServisi.RandevuDurumunuGuncelleAsync(id, Domain.Enumlar.RandevuDurumu.Onaylandi);
        TempData["Basari"] = "Randevu onaylandı.";
        
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
            
        return RedirectToAction(nameof(Randevular));
    }

    [HttpPost]
    public async Task<IActionResult> RandevuIptalEt(int id, string? returnUrl = null)
    {
        await _randevuServisi.RandevuDurumunuGuncelleAsync(id, Domain.Enumlar.RandevuDurumu.IptalEdildi);
        TempData["Basari"] = "Randevu iptal edildi.";
        
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
            
        return RedirectToAction(nameof(Randevular));
    }

    [HttpGet]
    public async Task<IActionResult> RandevuTakvimiVeri(DateTime start, DateTime end)
    {
        // FullCalendar start/end tarih aralığında tüm randevuları getir
        var tumRandevular = await _randevuServisi.TumRandevulariniGetirAsync();

        var filtered = tumRandevular
            .Where(r => r.Tarih.Date >= start.Date && r.Tarih.Date <= end.Date)
            .ToList();

        var events = filtered.Select(r => new
        {
            id = r.Id,
            title = $"{r.PersonelAdSoyad} - {r.Ucret:C0}",
            start = r.Tarih.Date.Add(r.Saat),
            end = r.Tarih.Date.Add(r.Saat).Add(TimeSpan.FromMinutes(45)),
            extendedProps = new
            {
                durum = r.Durum.ToString(),
                musteriId = r.MusteriId,
                personel = r.PersonelAdSoyad
            }
        });

        return Json(events);
    }

    #region Personel Yönetimi

  [HttpGet]
    public async Task<IActionResult> Personeller()
    {
     var personeller = await _personelServisi.TumPersonelleriGetirAsync();
        var items = new List<PersonelListeItemViewModel>();

        foreach (var p in personeller)
        {
     var kullanici = await _userManager.FindByIdAsync(p.KullaniciId);
      items.Add(new PersonelListeItemViewModel
     {
             Id = p.Id,
         Ad = p.Ad,
    Soyad = p.Soyad,
          KullaniciId = p.KullaniciId,
      KullaniciEposta = kullanici?.Email,
      AktifMi = p.AktifMi
   });
     }

        return View(items);
    }

    [HttpGet]
 public async Task<IActionResult> PersonelEkle()
    {
        var model = new PersonelDuzenleViewModel();
        await KullaniciListesiniDoldur(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PersonelEkle(PersonelDuzenleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await KullaniciListesiniDoldur(model);
            return View(model);
     }

        try
        {
            // Seçilen kullanıcının bilgilerini al
         var kullanici = await _userManager.FindByIdAsync(model.KullaniciId);
            if (kullanici == null)
      {
            ModelState.AddModelError(string.Empty, "Seçilen kullanıcı bulunamadı.");
   await KullaniciListesiniDoldur(model);
              return View(model);
     }

    var dto = new PersonelDto
            {
Ad = model.Ad,
   Soyad = model.Soyad,
         KullaniciId = model.KullaniciId,
    AktifMi = model.AktifMi
   };

            await _personelServisi.PersonelEkleAsync(dto);
  TempData["Basari"] = "Personel başarıyla eklendi.";
       return RedirectToAction(nameof(Personeller));
        }
        catch (Exception ex)
 {
        ModelState.AddModelError(string.Empty, $"Hata: {ex.Message}");
            await KullaniciListesiniDoldur(model);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> PersonelDuzenle(int id)
    {
        var personel = await _personelServisi.PersonelGetirAsync(id);
   if (personel == null)
            return NotFound();

   var vm = new PersonelDuzenleViewModel
     {
     Id = personel.Id,
  Ad = personel.Ad,
  Soyad = personel.Soyad,
            KullaniciId = personel.KullaniciId,
     AktifMi = personel.AktifMi
        };

   await KullaniciListesiniDoldur(vm);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> PersonelDuzenle(PersonelDuzenleViewModel model)
    {
        if (!ModelState.IsValid)
        {
     await KullaniciListesiniDoldur(model);
    return View(model);
        }

        try
  {
         // Seçilen kullanıcının bilgilerini al
         var kullanici = await _userManager.FindByIdAsync(model.KullaniciId);
    if (kullanici == null)
            {
    ModelState.AddModelError(string.Empty, "Seçilen kullanıcı bulunamadı.");
            await KullaniciListesiniDoldur(model);
       return View(model);
      }

            var dto = new PersonelDto
     {
                Id = model.Id,
    Ad = model.Ad,
        Soyad = model.Soyad,
                KullaniciId = model.KullaniciId,
                AktifMi = model.AktifMi
            };

            await _personelServisi.PersonelGuncelleAsync(dto);
   TempData["Basari"] = "Personel başarıyla güncellendi.";
            return RedirectToAction(nameof(Personeller));
      }
        catch (Exception ex)
        {
  ModelState.AddModelError(string.Empty, $"Hata: {ex.Message}");
      await KullaniciListesiniDoldur(model);
         return View(model);
  }
    }

    [HttpPost]
    public async Task<IActionResult> PersonelSil(int id)
    {
        try
        {
        await _personelServisi.PersonelSilAsync(id);
  TempData["Basari"] = "Personel başarıyla silindi.";
        }
        catch (Exception ex)
        {
    TempData["Hata"] = $"Personel silinemedi: {ex.Message}";
        }

        return RedirectToAction(nameof(Personeller));
    }

    [HttpPost]
    public async Task<IActionResult> PersonelAktiflikDegistir(int id)
    {
 var personel = await _personelServisi.PersonelGetirAsync(id);
        if (personel == null)
            return NotFound();

        personel.AktifMi = !personel.AktifMi;
        await _personelServisi.PersonelGuncelleAsync(personel);

      TempData["Basari"] = "Personel durumu güncellendi.";
     return RedirectToAction(nameof(Personeller));
    }

    /// <summary>
    /// Kullanıcı dropdown listesini doldurur
  /// </summary>
  private async Task KullaniciListesiniDoldur(PersonelDuzenleViewModel model)
    {
        var tumKullanicilar = await _userManager.Users
    .Where(u => u.AktifMi)
      .OrderBy(u => u.AdSoyad)
            .ToListAsync();

        model.KullaniciListesi = tumKullanicilar.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
  {
          Value = u.Id,
      Text = $"{u.AdSoyad} ({u.Email})"
        }).ToList();

        // Başta boş seçenek ekle
        model.KullaniciListesi.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
{
   Value = "",
            Text = "-- Kullanıcı Seçiniz --"
        });
    }

    #endregion

    #region Hizmet Yönetimi

    [HttpGet]
    public async Task<IActionResult> Hizmetler()
    {
      var hizmetler = await _hizmetServisi.TumHizmetleriGetirAsync();
   var items = hizmetler.Select(h => new HizmetListeItemViewModel
        {
            Id = h.Id,
  Ad = h.Ad,
       Sure = h.Sure,
      Ucret = h.Ucret,
    AktifMi = h.AktifMi
   }).ToList();

        return View(items);
    }

    [HttpGet]
    public IActionResult HizmetEkle()
    {
        return View(new HizmetDuzenleViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> HizmetEkle(HizmetDuzenleViewModel model)
    {
        if (!ModelState.IsValid)
    return View(model);

        try
 {
            var dto = new HizmetDto
   {
        Ad = model.Ad,
   Sure = model.Sure,
       Ucret = model.Ucret,
     AktifMi = model.AktifMi
            };

       await _hizmetServisi.HizmetEkleAsync(dto);
  TempData["Basari"] = "Hizmet başarıyla eklendi.";
   return RedirectToAction(nameof(Hizmetler));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Hata: {ex.Message}");
         return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> HizmetDuzenle(int id)
    {
        var hizmet = await _hizmetServisi.HizmetGetirAsync(id);
   if (hizmet == null)
   return NotFound();

        var vm = new HizmetDuzenleViewModel
     {
            Id = hizmet.Id,
     Ad = hizmet.Ad,
   Sure = hizmet.Sure,
      Ucret = hizmet.Ucret,
   AktifMi = hizmet.AktifMi
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> HizmetDuzenle(HizmetDuzenleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
     {
    var dto = new HizmetDto
            {
       Id = model.Id,
    Ad = model.Ad,
          Sure = model.Sure,
       Ucret = model.Ucret,
        AktifMi = model.AktifMi
          };

            await _hizmetServisi.HizmetGuncelleAsync(dto);
       TempData["Basari"] = "Hizmet başarıyla güncellendi.";
          return RedirectToAction(nameof(Hizmetler));
        }
        catch (Exception ex)
        {
     ModelState.AddModelError(string.Empty, $"Hata: {ex.Message}");
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> HizmetSil(int id)
    {
     try
        {
         await _hizmetServisi.HizmetSilAsync(id);
      TempData["Basari"] = "Hizmet başarıyla silindi.";
        }
      catch (Exception ex)
     {
            TempData["Hata"] = $"Hizmet silinemedi: {ex.Message}";
      }

  return RedirectToAction(nameof(Hizmetler));
}

    [HttpPost]
    public async Task<IActionResult> HizmetAktiflikDegistir(int id)
    {
    var hizmet = await _hizmetServisi.HizmetGetirAsync(id);
        if (hizmet == null)
            return NotFound();

        hizmet.AktifMi = !hizmet.AktifMi;
        await _hizmetServisi.HizmetGuncelleAsync(hizmet);

        TempData["Basari"] = "Hizmet durumu güncellendi.";
   return RedirectToAction(nameof(Hizmetler));
    }

    #endregion

    #region Salon Ayarları

    [HttpGet]
    public async Task<IActionResult> CalismaSaatleri()
  {
        var ayarlar = await _salonAyarlariServisi.CalismaSaatleriAyarlariniGetirAsync();
  
  var vm = new CalismaSaatleriAyarViewModel
        {
    BaslangicSaati = ayarlar.BaslangicSaati,
          BitisSaati = ayarlar.BitisSaati,
RandevuDilimiDakika = ayarlar.RandevuDilimiDakika
 };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CalismaSaatleri(CalismaSaatleriAyarViewModel model)
    {
      if (!ModelState.IsValid)
   return View(model);

        if (model.BitisSaati <= model.BaslangicSaati)
        {
            ModelState.AddModelError(string.Empty, "Bitiş saati başlangıç saatinden sonra olmalıdır.");
      return View(model);
        }

        try
        {
       var dto = new CalismaSaatleriAyarDto
            {
       BaslangicSaati = model.BaslangicSaati,
           BitisSaati = model.BitisSaati,
        RandevuDilimiDakika = model.RandevuDilimiDakika
        };

            await _salonAyarlariServisi.CalismaSaatleriAyarlariniKaydetAsync(dto);
     TempData["Basari"] = "Çalışma saatleri ayarları başarıyla kaydedildi.";
          return RedirectToAction(nameof(CalismaSaatleri));
   }
catch (Exception ex)
 {
            ModelState.AddModelError(string.Empty, $"Hata: {ex.Message}");
      return View(model);
        }
    }

    #endregion
}


