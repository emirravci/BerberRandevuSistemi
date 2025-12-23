using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BerberRandevu.Web.Models.Admin;

/// <summary>
/// Personel listesi görünümü için model.
/// </summary>
public class PersonelListeItemViewModel
{
  public int Id { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string AdSoyad => $"{Ad} {Soyad}";
    public bool AktifMi { get; set; }
    public string KullaniciId { get; set; } = null!;
    public string? KullaniciEposta { get; set; }
}

/// <summary>
/// Personel ekleme/düzenleme formu için model.
/// </summary>
public class PersonelDuzenleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad alaný zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = null!;

    [Required(ErrorMessage = "Soyad alaný zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    [Display(Name = "Soyad")]
  public string Soyad { get; set; } = null!;

    [Required(ErrorMessage = "Kullanýcý seçimi zorunludur.")]
  [Display(Name = "Kullanýcý")]
    public string KullaniciId { get; set; } = null!;

    [Display(Name = "Aktif Mi?")]
    public bool AktifMi { get; set; } = true;

    /// <summary>
    /// Dropdown için kullanýcý listesi
    /// </summary>
    public List<SelectListItem> KullaniciListesi { get; set; } = new();
}
