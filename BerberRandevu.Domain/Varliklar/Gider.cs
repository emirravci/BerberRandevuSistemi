using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// İşletmeye ait gider kalemlerini temsil eder.
/// </summary>
public class Gider : TemelVarlik
{
    public string Baslik { get; set; } = null!;

    public decimal Tutar { get; set; }

    public DateTime Tarih { get; set; }

    public string? Aciklama { get; set; }
}


