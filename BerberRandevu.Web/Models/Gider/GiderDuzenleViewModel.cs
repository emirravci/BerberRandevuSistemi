using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Gider;

public class GiderDuzenleViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = null!;

    [Required]
    [Display(Name = "Tutar")]
    public decimal Tutar { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Tarih")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }
}


