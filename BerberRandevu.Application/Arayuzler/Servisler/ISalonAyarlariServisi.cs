using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Application.Arayuzler.Servisler;

/// <summary>
/// Salon ayarlarý yönetimi için servis sözleþmesi.
/// </summary>
public interface ISalonAyarlariServisi
{
    Task<CalismaSaatleriAyarDto> CalismaSaatleriAyarlariniGetirAsync();
    Task CalismaSaatleriAyarlariniKaydetAsync(CalismaSaatleriAyarDto dto);
  Task<string?> AyarGetirAsync(string anahtar);
    Task AyarKaydetAsync(string anahtar, string deger, string? aciklama = null);
}
