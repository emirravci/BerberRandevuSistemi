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
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly BerberDbContext _dbContext;

    public RandevuController(
        IRandevuServisi randevuServisi,
        IPersonelServisi personelServisi,
        UserManager<UygulamaKullanicisi> userManager,
        BerberDbContext dbContext)
    {
        _randevuServisi = randevuServisi;
        _personelServisi = personelServisi;
        _userManager = userManager;
        _dbContext = dbContext;
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


