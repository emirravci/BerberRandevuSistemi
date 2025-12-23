using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Admin;

/// <summary>
/// Salon çalýþma saatleri ayarlarý için ViewModel.
/// </summary>
public class CalismaSaatleriAyarViewModel
{
 [Required(ErrorMessage = "Baþlangýç saati zorunludur.")]
    [Display(Name = "Çalýþma Baþlangýç Saati")]
 public TimeSpan BaslangicSaati { get; set; } = new TimeSpan(9, 0, 0);

    [Required(ErrorMessage = "Bitiþ saati zorunludur.")]
    [Display(Name = "Çalýþma Bitiþ Saati")]
    public TimeSpan BitisSaati { get; set; } = new TimeSpan(20, 0, 0);

  [Required(ErrorMessage = "Randevu dilimi zorunludur.")]
    [Range(5, 120, ErrorMessage = "Randevu dilimi 5 ile 120 dakika arasýnda olmalýdýr.")]
[Display(Name = "Randevu Zaman Dilimi (Dakika)")]
    public int RandevuDilimiDakika { get; set; } = 30;

    public string BaslangicSaatiText => BaslangicSaati.ToString(@"hh\:mm");
    public string BitisSaatiText => BitisSaati.ToString(@"hh\:mm");
}
