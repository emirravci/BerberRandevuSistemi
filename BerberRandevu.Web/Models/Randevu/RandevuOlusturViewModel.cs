using System.ComponentModel.DataAnnotations;
using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Web.Models.Randevu;

public class RandevuOlusturViewModel
{
    [Required(ErrorMessage = "Personel seçimi zorunludur.")]
    [Display(Name = "Personel")]
    public int PersonelId { get; set; }

    [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
    [Display(Name = "Hizmet")]
    public int HizmetId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Tarih")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Saat")]
    public TimeSpan Saat { get; set; }

    [Display(Name = "Ücret")]
    public decimal Ucret { get; set; }

    public List<PersonelDto> Personeller { get; set; } = new();
    public List<HizmetDto> Hizmetler { get; set; } = new();
}


