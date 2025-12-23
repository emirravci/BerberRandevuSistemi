using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// Salon genelindeki çalýþma saatleri ve randevu ayarlarýný tutar.
/// </summary>
public class SalonAyarlari : TemelVarlik
{
    /// <summary>
  /// Ayar anahtarý (örn: "CalismaSaatBaslangic", "CalismaSaatBitis", "RandevuDilimi")
    /// </summary>
    public string Anahtar { get; set; } = null!;

    /// <summary>
    /// Ayar deðeri
    /// </summary>
    public string Deger { get; set; } = null!;

    /// <summary>
    /// Ayar açýklamasý
    /// </summary>
    public string? Aciklama { get; set; }
}
