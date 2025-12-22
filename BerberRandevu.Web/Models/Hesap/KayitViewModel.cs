using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Hesap;

public class KayitViewModel
{
    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Eposta { get; set; } = null!;

    [Required]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Sifre { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Şifre (Tekrar)")]
    [Compare(nameof(Sifre), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string SifreTekrar { get; set; } = null!;
}


