using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// Randevuya ait ödeme kaydını temsil eder.
/// </summary>
public class Odeme : TemelVarlik
{
    public int RandevuId { get; set; }

    public decimal Tutar { get; set; }

    public DateTime OdemeTarihi { get; set; }

    /// <summary>
    /// Nakit, Kart vb. bilgileri tutmak için serbest metin.
    /// İleride enum ya da ayrı tabloya dönüştürülebilir.
    /// </summary>
    public string OdemeTipi { get; set; } = null!;

    public Randevu Randevu { get; set; } = null!;
}


