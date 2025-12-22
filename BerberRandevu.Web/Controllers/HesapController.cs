using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Web.Models.Hesap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Kullanıcı kayıt ve giriş işlemlerini yöneten controller.
/// </summary>
public class HesapController : Controller
{
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly SignInManager<UygulamaKullanicisi> _signInManager;

    public HesapController(
        UserManager<UygulamaKullanicisi> userManager,
        SignInManager<UygulamaKullanicisi> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Giris(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new GirisViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Giris(GirisViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Eposta);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Sifre,
            model.BeniHatirla,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Kayit()
    {
        return View(new KayitViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Kayit(KayitViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new UygulamaKullanicisi
        {
            UserName = model.Eposta,
            Email = model.Eposta,
            AdSoyad = model.AdSoyad
        };

        var result = await _userManager.CreateAsync(user, model.Sifre);
        if (result.Succeeded)
        {
            // Varsayılan olarak kullanıcı rolü ver.
            await _userManager.AddToRoleAsync(user, "Kullanıcı");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cikis()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}


