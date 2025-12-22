using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Infrastructure.Depolar;
using BerberRandevu.Infrastructure.VeriErisim;

namespace BerberRandevu.Infrastructure.BirimIs;

/// <summary>
/// Repository erişimlerini merkezi olarak yöneten çalışma birimi implementasyonu.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly BerberDbContext _dbContext;

    public UnitOfWork(
        BerberDbContext dbContext,
        IRandevuRepository randevuRepository,
        IPersonelRepository personelRepository)
    {
        _dbContext = dbContext;
        RandevuDeposu = randevuRepository;
        PersonelDeposu = personelRepository;
    }

    public IRandevuRepository RandevuDeposu { get; }
    public IPersonelRepository PersonelDeposu { get; }

    public async Task<int> KaydetAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}


