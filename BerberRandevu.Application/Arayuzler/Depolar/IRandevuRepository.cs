using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Arayuzler.Depolar;

/// <summary>
/// Randevuya özel sorgular ve işlemler için repository arayüzü.
/// </summary>
public interface IRandevuRepository : IGenericRepository<Randevu>
{
    Task<bool> PersonelIcinZamanAraligindaRandevuVarMiAsync(
        int personelId,
        DateTime tarih,
        TimeSpan saat);

    Task<IReadOnlyList<Randevu>> PersonelRandevulariniGetirAsync(
        int personelId,
        DateTime? baslangicTarihi = null,
        DateTime? bitisTarihi = null);
}


