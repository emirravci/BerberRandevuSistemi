using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Yönetici paneli ve gelir-gider raporlarını yönetir.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IOdemeServisi _odemeServisi;
    private readonly IGiderServisi _giderServisi;

    public AdminController(IOdemeServisi odemeServisi, IGiderServisi giderServisi)
    {
        _odemeServisi = odemeServisi;
        _giderServisi = giderServisi;
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
}


