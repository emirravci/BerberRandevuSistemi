using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Admin;

public class KullaniciListeItemViewModel
{
    public string Id { get; set; } = null!;

    [Display(Name = "E-posta")]
    public string? Eposta { get; set; }

    [Display(Name = "Ad Soyad")]
    public string? AdSoyad { get; set; }

    [Display(Name = "Roller")]
    public string Roller { get; set; } = string.Empty;

    [Display(Name = "Aktif mi?")]
    public bool AktifMi { get; set; }
}

public class KullaniciRolGuncelleViewModel
{
    public string Id { get; set; } = null!;

    [Display(Name = "Rol")]
    public string Rol { get; set; } = null!;
}


