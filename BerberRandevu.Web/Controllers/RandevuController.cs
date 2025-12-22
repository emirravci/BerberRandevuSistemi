using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Web.Models.Randevu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Kullanıcı randevu oluşturma ve kendi randevularını görüntüleme işlemlerini yönetir.
/// </summary>
[Authorize(Roles = "Kullanıcı")]
public class RandevuController : Controller
{
    private readonly IRandevuServisi _randevuServisi;
    private readonly IPersonelServisi _personelServisi;
    private readonly UserManager<UygulamaKullanicisi> _userManager;

    public RandevuController(
        IRandevuServisi randevuServisi,
        IPersonelServisi personelServisi,
        UserManager<UygulamaKullanicisi> userManager)
    {
        _randevuServisi = randevuServisi;
        _personelServisi = personelServisi;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Olustur()
    {
        var vm = new RandevuOlusturViewModel
        {
            Personeller = (await _personelServisi.AktifPersonelleriGetirAsync()).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Olustur(RandevuOlusturViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Personeller = (await _personelServisi.AktifPersonelleriGetirAsync()).ToList();
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
}


