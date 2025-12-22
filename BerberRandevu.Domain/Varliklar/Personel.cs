using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Domain.Varliklar;

/// <summary>
/// Berber salonunda çalışan personel bilgisini temsil eder.
/// </summary>
public class Personel : TemelVarlik
{
    public string KullaniciId { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public bool AktifMi { get; set; } = true;

    public ICollection<CalismaSaati> CalismaSaatleri { get; set; } = new List<CalismaSaati>();

    public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
}


