namespace BerberRandevu.Application.DTOlar;

/// <summary>
/// Gider kayıtları için kullanılan DTO.
/// </summary>
public class GiderDto
{
    public int Id { get; set; }

    public string Baslik { get; set; } = null!;

    public decimal Tutar { get; set; }

    public DateTime Tarih { get; set; }

    public string? Aciklama { get; set; }
}


