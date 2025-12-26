namespace BerberRandevu.Application.DTOlar;

/// <summary>
/// Salon ayarlarý için DTO.
/// </summary>
public class SalonAyarlariDto
{
    public int Id { get; set; }
    public string Anahtar { get; set; } = null!;
    public string Deger { get; set; } = null!;
    public string? Aciklama { get; set; }
}

/// <summary>
/// Çalýþma saatleri yönetimi için toplu DTO.
/// </summary>
public class CalismaSaatleriAyarDto
{
    public TimeSpan BaslangicSaati { get; set; } = new TimeSpan(9, 0, 0);  // 09:00
    public TimeSpan BitisSaati { get; set; } = new TimeSpan(20, 0, 0);      // 20:00
    public int RandevuDilimiDakika { get; set; } = 30; // Varsayýlan 30 dakika
    public List<GunlukCalismaSaatiDto> GunlukSaatler { get; set; } = new();
}

/// <summary>
/// Günlük çalýþma saati DTO
/// </summary>
public class GunlukCalismaSaatiDto
{
    public DayOfWeek Gun { get; set; }
    public bool AcikMi { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
}
