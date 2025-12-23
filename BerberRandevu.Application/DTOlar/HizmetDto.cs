namespace BerberRandevu.Application.DTOlar;

/// <summary>
/// Hizmet iþlemleri için kullanýlan DTO.
/// </summary>
public class HizmetDto
{
    public int Id { get; set; }

    public string Ad { get; set; } = null!;

    /// <summary>
    /// Hizmetin süresi (dakika cinsinden).
    /// </summary>
    public int Sure { get; set; }

    /// <summary>
    /// Hizmetin ücreti (TL cinsinden).
    /// </summary>
    public decimal Ucret { get; set; }

    public bool AktifMi { get; set; }
}
