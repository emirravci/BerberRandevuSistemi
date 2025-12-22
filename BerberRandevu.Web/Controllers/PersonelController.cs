using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Domain.Enumlar;
using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Domain.Varliklar;
using BerberRandevu.Infrastructure.VeriErisim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Web.Controllers;

/// <summary>
/// Personel tarafı için randevu onay ve listeleme işlemlerini yönetir.
/// </summary>
[Authorize(Roles = "Personel")]
public class PersonelController : Controller
{
    private readonly IRandevuServisi _randevuServisi;
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly BerberDbContext _dbContext;

    public PersonelController(
        IRandevuServisi randevuServisi,
        UserManager<UygulamaKullanicisi> userManager,
        BerberDbContext dbContext)
    {
        _randevuServisi = randevuServisi;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    // Örnek olarak personel Id'si claim veya ayrı tabloda tutulabilir; basitlik için sabit atanmış kabul edilebilir.
    // Gerçek senaryoda Personel tablosu ile kullanıcı eşleştirmesi yapılmalıdır.

    [HttpGet]
    public async Task<IActionResult> Bekleyen()
    {
        var userId = _userManager.GetUserId(User)!;
        var personel = await _dbContext.Personeller
            .FirstOrDefaultAsync(p => p.KullaniciId == userId && p.AktifMi);

        if (personel == null)
            return View(new List<BerberRandevu.Application.DTOlar.RandevuDto>());

        int personelId = personel.Id;
        var randevular = await _randevuServisi.PersonelRandevulariniGetirAsync(personelId, DateTime.Today);
        return View(randevular.Where(r => r.Durum == RandevuDurumu.Beklemede).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Onayla(int id, string? returnUrl = null)
    {
        await _randevuServisi.RandevuDurumunuGuncelleAsync(id, RandevuDurumu.Onaylandi);
        TempData["Basari"] = "Randevu onaylandı.";
        
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
            
        return RedirectToAction(nameof(Bekleyen));
    }

    [HttpPost]
    public async Task<IActionResult> IptalEt(int id, string? returnUrl = null)
    {
        await _randevuServisi.RandevuDurumunuGuncelleAsync(id, RandevuDurumu.IptalEdildi);
        TempData["Basari"] = "Randevu iptal edildi.";
        
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
            
        return RedirectToAction(nameof(Bekleyen));
    }

    [HttpGet]
    public IActionResult Takvim()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> RandevuTakvimiVeri(DateTime start, DateTime end)
    {
        var userId = _userManager.GetUserId(User)!;
        var personel = await _dbContext.Personeller
            .FirstOrDefaultAsync(p => p.KullaniciId == userId && p.AktifMi);

        if (personel == null)
            return Json(Array.Empty<object>());

        int personelId = personel.Id;
        var randevular = await _randevuServisi.PersonelRandevulariniGetirAsync(personelId);

        var filtered = randevular
            .Where(r => r.Tarih.Date >= start.Date && r.Tarih.Date <= end.Date)
            .ToList();

        var events = filtered.Select(r => new
        {
            id = r.Id,
            title = $"{r.MusteriId} - {r.Ucret:C0}",
            start = r.Tarih.Date.Add(r.Saat),
            end = r.Tarih.Date.Add(r.Saat).Add(TimeSpan.FromMinutes(45)),
            extendedProps = new
            {
                durum = r.Durum.ToString(),
                musteriId = r.MusteriId
            }
        });

        return Json(events);
    }

    [HttpPost]
    public async Task<IActionResult> CalismaSaatiEkle(DateTime start, DateTime end)
    {
        var userId = _userManager.GetUserId(User)!;
        var personel = await _dbContext.Personeller
            .FirstOrDefaultAsync(p => p.KullaniciId == userId && p.AktifMi);

        if (personel == null)
            return BadRequest("Personel bulunamadı.");

        int personelId = personel.Id;

        var gun = start.DayOfWeek;
        var baslangic = start.TimeOfDay;
        var bitis = end.TimeOfDay;

        if (bitis <= baslangic)
            bitis = baslangic.Add(TimeSpan.FromMinutes(30));

        var entity = new CalismaSaati
        {
            PersonelId = personelId,
            Gun = gun,
            BaslangicSaati = baslangic,
            BitisSaati = bitis
        };

        await _dbContext.CalismaSaatleri.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> CalismaSaatleriVeri()
    {
        var userId = _userManager.GetUserId(User)!;
        var personel = await _dbContext.Personeller
            .FirstOrDefaultAsync(p => p.KullaniciId == userId && p.AktifMi);

        if (personel == null)
            return Json(Array.Empty<object>());

        int personelId = personel.Id;

        var saatler = await _dbContext.CalismaSaatleri
            .Where(c => c.PersonelId == personelId)
            .ToListAsync();

        var events = new List<object>();

        var bugun = DateTime.Today;
        var haftaBaslangic = bugun.AddDays(-(int)bugun.DayOfWeek);

        foreach (var cs in saatler)
        {
            var gunTarihi = haftaBaslangic.AddDays((int)cs.Gun);
            var start = gunTarihi.Date.Add(cs.BaslangicSaati);
            var end = gunTarihi.Date.Add(cs.BitisSaati);

            events.Add(new
            {
                title = "Çalışma Saatleri",
                start,
                end,
                display = "background",
                backgroundColor = "#1d4ed8",
                borderColor = "#1d4ed8"
            });
        }

        return Json(events);
    }
}


