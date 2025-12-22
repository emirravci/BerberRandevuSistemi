using System.ComponentModel.DataAnnotations;
using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Web.Models.Randevu;

public class RandevuOlusturViewModel
{
    [Required]
    [Display(Name = "Personel")]
    public int PersonelId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Tarih")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Saat")]
    public TimeSpan Saat { get; set; }

    [Required]
    [Display(Name = "Ücret")]
    public decimal Ucret { get; set; }

    public List<PersonelDto> Personeller { get; set; } = new();
}


