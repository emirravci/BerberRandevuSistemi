using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Domain.Varliklar;
using BerberRandevu.Infrastructure.VeriErisim;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Infrastructure.Depolar;

/// <summary>
/// Randevuya özel sorgular için repository implementasyonu.
/// </summary>
public class RandevuRepository : GenericRepository<Randevu>, IRandevuRepository
{
    public RandevuRepository(BerberDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> PersonelIcinZamanAraligindaRandevuVarMiAsync(
        int personelId,
        DateTime tarih,
        TimeSpan saat)
    {
        var dateOnly = tarih.Date;

        return await _dbSet.AnyAsync(r =>
            !r.SilindiMi &&
            r.PersonelId == personelId &&
            r.Tarih.Date == dateOnly &&
            r.Saat == saat);
    }

    public async Task<IReadOnlyList<Randevu>> PersonelRandevulariniGetirAsync(
        int personelId,
        DateTime? baslangicTarihi = null,
        DateTime? bitisTarihi = null)
    {
        var sorgu = _dbSet
            .Include(r => r.Personel)
            .Where(r => !r.SilindiMi && r.PersonelId == personelId);

        if (baslangicTarihi.HasValue)
        {
            var d = baslangicTarihi.Value.Date;
            sorgu = sorgu.Where(r => r.Tarih.Date >= d);
        }

        if (bitisTarihi.HasValue)
        {
            var d = bitisTarihi.Value.Date;
            sorgu = sorgu.Where(r => r.Tarih.Date <= d);
        }

        return await sorgu
            .OrderBy(r => r.Tarih)
            .ThenBy(r => r.Saat)
            .ToListAsync();
    }
}


