using BerberRandevu.Application.Arayuzler.Servisler;
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
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        IOdemeServisi odemeServisi,
        IGiderServisi giderServisi,
        IRandevuServisi randevuServisi,
        UserManager<UygulamaKullanicisi> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _odemeServisi = odemeServisi;
        _giderServisi = giderServisi;
        _randevuServisi = randevuServisi;
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
}


