using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Enumlar;

namespace BerberRandevu.Application.Arayuzler.Servisler;

/// <summary>
/// Randevu iş kurallarını yöneten servis sözleşmesi.
/// </summary>
public interface IRandevuServisi
{
    /// <summary>
    /// Kullanıcı için yeni bir randevu oluşturur.
    /// İş kuralları:
    /// - Aynı personel aynı tarih/saatte iki randevu alamaz.
    /// - Personelin çalışma saati dışında randevu oluşturulamaz.
    /// </summary>
    Task<RandevuDto> RandevuOlusturAsync(RandevuDto randevuDto);

    /// <summary>
    /// Randevunun durumunu günceller (Onayla, İptal et, Tamamla vb.).
    /// - Onaylanmayan randevu için ödeme alınamaz (iş kuralı ödeme servisinde tekrar kontrol edilir).
    /// </summary>
    Task RandevuDurumunuGuncelleAsync(int randevuId, RandevuDurumu yeniDurum);

    Task<IReadOnlyList<RandevuDto>> KullaniciRandevulariniGetirAsync(string musteriId);

    Task<IReadOnlyList<RandevuDto>> PersonelRandevulariniGetirAsync(int personelId, DateTime? tarih = null);
}


