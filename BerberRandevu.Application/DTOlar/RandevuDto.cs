using BerberRandevu.Domain.Enumlar;

namespace BerberRandevu.Application.DTOlar;

/// <summary>
/// Randevu oluşturma ve listeleme senaryolarında kullanılan DTO.
/// </summary>
public class RandevuDto
{
    public int Id { get; set; }

    public string MusteriId { get; set; } = null!;

    public int PersonelId { get; set; }

    public DateTime Tarih { get; set; }

    public TimeSpan Saat { get; set; }

    public RandevuDurumu Durum { get; set; }

    public decimal Ucret { get; set; }

    public bool OdemeAlindiMi { get; set; }

    public string? PersonelAdSoyad { get; set; }
}


