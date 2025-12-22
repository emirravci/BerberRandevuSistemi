using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Application.Arayuzler.Servisler;

/// <summary>
/// Ödeme alma ve raporlama işlemleri için servis sözleşmesi.
/// </summary>
public interface IOdemeServisi
{
    /// <summary>
    /// Belirli bir randevu için ödeme alır ve gelir kaydı oluşturur.
    /// İş kuralı: Sadece Onaylandi veya Tamamlandi durumundaki randevular için ödeme alınabilir.
    /// </summary>
    Task<OdemeDto> OdemeAlAsync(OdemeDto dto);

    Task<IReadOnlyList<OdemeDto>> TarihAraligindaOdemeleriGetirAsync(DateTime baslangic, DateTime bitis);
}


