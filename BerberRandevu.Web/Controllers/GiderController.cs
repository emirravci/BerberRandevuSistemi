using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Web.Models.Gider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Gider kayıtlarını yöneten controller.
/// </summary>
[Authorize(Roles = "Admin")]
public class GiderController : Controller
{
    private readonly IGiderServisi _giderServisi;

    public GiderController(IGiderServisi giderServisi)
    {
        _giderServisi = giderServisi;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var baslangic = DateTime.Today.AddMonths(-1);
        var bitis = DateTime.Today;
        var giderler = await _giderServisi.TarihAraligindaGiderleriGetirAsync(baslangic, bitis);
        return View(giderler);
    }

    [HttpGet]
    public IActionResult Ekle()
    {
        return View(new GiderDuzenleViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(GiderDuzenleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new GiderDto
        {
            Baslik = model.Baslik,
            Tutar = model.Tutar,
            Tarih = model.Tarih,
            Aciklama = model.Aciklama
        };

        await _giderServisi.GiderEkleAsync(dto);
        TempData["Basari"] = "Gider kaydı eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Duzenle(int id)
    {
        var liste = await _giderServisi.TarihAraligindaGiderleriGetirAsync(DateTime.MinValue, DateTime.MaxValue);
        var dto = liste.FirstOrDefault(g => g.Id == id);
        if (dto == null)
            return NotFound();

        var vm = new GiderDuzenleViewModel
        {
            Id = dto.Id,
            Baslik = dto.Baslik,
            Tutar = dto.Tutar,
            Tarih = dto.Tarih,
            Aciklama = dto.Aciklama
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Duzenle(GiderDuzenleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new GiderDto
        {
            Id = model.Id,
            Baslik = model.Baslik,
            Tutar = model.Tutar,
            Tarih = model.Tarih,
            Aciklama = model.Aciklama
        };

        await _giderServisi.GiderGuncelleAsync(dto);
        TempData["Basari"] = "Gider kaydı güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Sil(int id)
    {
        await _giderServisi.GiderSilAsync(id);
        TempData["Basari"] = "Gider kaydı silindi.";
        return RedirectToAction(nameof(Index));
    }
}


