using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Odeme;

public class OdemeAlViewModel
{
    [Required]
    [Display(Name = "Randevu Id")]
    public int RandevuId { get; set; }

    [Required]
    [Display(Name = "Tutar")]
    public decimal Tutar { get; set; }

    [Required]
    [Display(Name = "Ödeme Tipi")]
    public string OdemeTipi { get; set; } = null!;
}


