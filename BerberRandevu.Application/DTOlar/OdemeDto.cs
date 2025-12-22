namespace BerberRandevu.Application.DTOlar;

/// <summary>
/// Ödeme kayıtları için kullanılan DTO.
/// </summary>
public class OdemeDto
{
    public int Id { get; set; }

    public int RandevuId { get; set; }

    public decimal Tutar { get; set; }

    public DateTime OdemeTarihi { get; set; }

    public string OdemeTipi { get; set; } = null!;
}


