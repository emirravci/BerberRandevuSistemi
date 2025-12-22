using BerberRandevu.Application.Arayuzler.Depolar;

namespace BerberRandevu.Application.Arayuzler.BirimIs;

/// <summary>
/// Repository örneklerini tek bir çalışma birimi altında toplayan yapı.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IRandevuRepository RandevuDeposu { get; }
    IPersonelRepository PersonelDeposu { get; }

    Task<int> KaydetAsync(CancellationToken cancellationToken = default);
}


