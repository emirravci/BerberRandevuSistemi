using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Domain.Enumlar;
using BerberRandevu.Domain.Kullanicilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Personel tarafı için randevu onay ve listeleme işlemlerini yönetir.
/// </summary>
[Authorize(Roles = "Personel")]
public class PersonelController : Controller
{
    private readonly IRandevuServisi _randevuServisi;
    private readonly UserManager<UygulamaKullanicisi> _userManager;

    public PersonelController(IRandevuServisi randevuServisi, UserManager<UygulamaKullanicisi> userManager)
    {
        _randevuServisi = randevuServisi;
        _userManager = userManager;
    }

    // Örnek olarak personel Id'si claim veya ayrı tabloda tutulabilir; basitlik için sabit atanmış kabul edilebilir.
    // Gerçek senaryoda Personel tablosu ile kullanıcı eşleştirmesi yapılmalıdır.

    [HttpGet]
    public async Task<IActionResult> Bekleyen()
    {
        // Demo: Tüm personel için bugünkü randevuları listelemek yerine,
        // tek örnek personel üzerinden çalıştığımızı varsayıyoruz.
        int personelId = 1;
        var randevular = await _randevuServisi.PersonelRandevulariniGetirAsync(personelId, DateTime.Today);
        return View(randevular.Where(r => r.Durum == RandevuDurumu.Beklemede).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Onayla(int id)
    {
        await _randevuServisi.RandevuDurumunuGuncelleAsync(id, RandevuDurumu.Onaylandi);
        return RedirectToAction(nameof(Bekleyen));
    }

    [HttpPost]
    public async Task<IActionResult> IptalEt(int id)
    {
        await _randevuServisi.RandevuDurumunuGuncelleAsync(id, RandevuDurumu.IptalEdildi);
        return RedirectToAction(nameof(Bekleyen));
    }
}


