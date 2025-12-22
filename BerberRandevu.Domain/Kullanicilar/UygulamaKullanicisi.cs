using Microsoft.AspNetCore.Identity;

namespace BerberRandevu.Domain.Kullanicilar;

/// <summary>
/// Uygulama içinde kullanılacak Identity tabanlı kullanıcı varlığı.
/// </summary>
public class UygulamaKullanicisi : IdentityUser
{
    /// <summary>
    /// Kullanıcının görünen adı (Müşteri adı veya personel adı olarak kullanılabilir).
    /// </summary>
    public string? AdSoyad { get; set; }

    /// <summary>
    /// Soft delete senaryoları için kullanılabilir.
    /// </summary>
    public bool AktifMi { get; set; } = true;
}


