using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// Berber salonunda sunulan hizmetleri temsil eder.
/// </summary>
public class Hizmet : TemelVarlik
{
    public string Ad { get; set; } = null!;

    /// <summary>
    /// Hizmetin süresi (dakika cinsinden).
    /// </summary>
    public int Sure { get; set; }

    /// <summary>
    /// Hizmetin ücreti (TL cinsinden).
    /// </summary>
    public decimal Ucret { get; set; }

    public bool AktifMi { get; set; } = true;
}
