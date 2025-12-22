namespace BerberRandevu.Application.DTOlar;

/// <summary>
/// Personel işlemleri için kullanılan DTO.
/// </summary>
public class PersonelDto
{
    public int Id { get; set; }

    public string KullaniciId { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public bool AktifMi { get; set; }
}


