using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Hesap;

public class GirisViewModel
{
    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Eposta { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Sifre { get; set; } = null!;

    [Display(Name = "Beni Hatırla")]
    public bool BeniHatirla { get; set; }
}


