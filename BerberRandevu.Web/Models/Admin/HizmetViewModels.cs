using System.ComponentModel.DataAnnotations;

namespace BerberRandevu.Web.Models.Admin;

/// <summary>
/// Hizmet listesi görünümü için model.
/// </summary>
public class HizmetListeItemViewModel
{
    public int Id { get; set; }
    public string Ad { get; set; } = null!;
    public int Sure { get; set; }
    public string SureText => $"{Sure} dakika";
    public decimal Ucret { get; set; }
    public bool AktifMi { get; set; }
}

/// <summary>
/// Hizmet ekleme/düzenleme formu için model.
/// </summary>
public class HizmetDuzenleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hizmet adý zorunludur.")]
    [StringLength(100, ErrorMessage = "Hizmet adý en fazla 100 karakter olabilir.")]
    [Display(Name = "Hizmet Adý")]
    public string Ad { get; set; } = null!;

    [Required(ErrorMessage = "Hizmet süresi zorunludur.")]
    [Range(5, 480, ErrorMessage = "Hizmet süresi 5 ile 480 dakika arasýnda olmalýdýr.")]
    [Display(Name = "Hizmet Süresi (Dakika)")]
    public int Sure { get; set; } = 30;

    [Required(ErrorMessage = "Hizmet ücreti zorunludur.")]
    [Range(0.01, 10000, ErrorMessage = "Hizmet ücreti 0.01 ile 10000 TL arasýnda olmalýdýr.")]
    [Display(Name = "Hizmet Ücreti (TL)")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal Ucret { get; set; }

    [Display(Name = "Aktif Mi?")]
    public bool AktifMi { get; set; } = true;
}
