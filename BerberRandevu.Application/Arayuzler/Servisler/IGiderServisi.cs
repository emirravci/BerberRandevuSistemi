using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Application.Arayuzler.Servisler;

/// <summary>
/// Gider yönetimi ve raporlama işlemleri için servis sözleşmesi.
/// </summary>
public interface IGiderServisi
{
    Task<GiderDto> GiderEkleAsync(GiderDto dto);

    Task<GiderDto> GiderGuncelleAsync(GiderDto dto);

    Task GiderSilAsync(int id);

    Task<IReadOnlyList<GiderDto>> TarihAraligindaGiderleriGetirAsync(DateTime baslangic, DateTime bitis);
}


