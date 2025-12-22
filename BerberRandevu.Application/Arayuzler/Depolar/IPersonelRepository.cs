using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Arayuzler.Depolar;

/// <summary>
/// Personel varlığına özel ek sorgular için repository arayüzü.
/// </summary>
public interface IPersonelRepository : IGenericRepository<Personel>
{
    Task<IReadOnlyList<Personel>> AktifPersonelleriGetirAsync();
}


