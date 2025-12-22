using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// Personelin belirli bir gün için çalışma saat aralığını temsil eder.
/// </summary>
public class CalismaSaati : TemelVarlik
{
    public int PersonelId { get; set; }

    public DayOfWeek Gun { get; set; }

    public TimeSpan BaslangicSaati { get; set; }

    public TimeSpan BitisSaati { get; set; }

    public Personel Personel { get; set; } = null!;
}


