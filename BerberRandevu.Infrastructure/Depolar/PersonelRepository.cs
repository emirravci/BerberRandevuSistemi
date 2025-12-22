using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Domain.Varliklar;
using BerberRandevu.Infrastructure.VeriErisim;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Infrastructure.Depolar;

/// <summary>
/// Personel varlığına özel sorgular için repository implementasyonu.
/// </summary>
public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
{
    public PersonelRepository(BerberDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Personel>> AktifPersonelleriGetirAsync()
    {
        return await _dbSet
            .Where(p => !p.SilindiMi && p.AktifMi)
            .OrderBy(p => p.Ad)
            .ThenBy(p => p.Soyad)
            .ToListAsync();
    }
}


