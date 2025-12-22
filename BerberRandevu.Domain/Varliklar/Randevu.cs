using BerberRandevu.Domain.Enumlar;
using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// Müşteri ile personel arasındaki randevu kaydını temsil eder.
/// </summary>
public class Randevu : TemelVarlik
{
    public string MusteriId { get; set; } = null!;

    public int PersonelId { get; set; }

    public DateTime Tarih { get; set; }

    /// <summary>
    /// Randevu başlangıç saati (Tarih ile birleşik de tutulabilir, burada sadeleştirme için ayrıştırıldı).
    /// </summary>
    public TimeSpan Saat { get; set; }

    public RandevuDurumu Durum { get; set; } = RandevuDurumu.Beklemede;

    public decimal Ucret { get; set; }

    public bool OdemeAlindiMi { get; set; }

    public Personel Personel { get; set; } = null!;

    // Navigasyon: İleride Musteri navigasyonu Identity üzerinden kurulabilir.
    public Odeme? Odeme { get; set; }
}


