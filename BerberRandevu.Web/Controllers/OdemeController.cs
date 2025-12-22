using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Web.Models.Odeme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Randevular için ödeme alma işlemlerini yönetir.
/// </summary>
[Authorize(Roles = "Admin,Personel")]
public class OdemeController : Controller
{
    private readonly IOdemeServisi _odemeServisi;

    public OdemeController(IOdemeServisi odemeServisi)
    {
        _odemeServisi = odemeServisi;
    }

    [HttpGet]
    public IActionResult OdemeAl(int randevuId)
    {
        var model = new OdemeAlViewModel
        {
            RandevuId = randevuId
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> OdemeAl(OdemeAlViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var dto = new OdemeDto
            {
                RandevuId = model.RandevuId,
                Tutar = model.Tutar,
                OdemeTipi = model.OdemeTipi,
                OdemeTarihi = DateTime.UtcNow
            };

            await _odemeServisi.OdemeAlAsync(dto);
            TempData["Basari"] = "Ödeme başarıyla alındı.";
            return RedirectToAction("Index", "Admin");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}


