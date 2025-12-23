using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Application.Arayuzler.Servisler;

/// <summary>
/// Hizmet yönetimi için servis sözleþmesi.
/// </summary>
public interface IHizmetServisi
{
    Task<IReadOnlyList<HizmetDto>> TumHizmetleriGetirAsync();

    Task<IReadOnlyList<HizmetDto>> AktifHizmetleriGetirAsync();

    Task<HizmetDto?> HizmetGetirAsync(int id);

    Task<HizmetDto> HizmetEkleAsync(HizmetDto dto);

    Task<HizmetDto> HizmetGuncelleAsync(HizmetDto dto);

    Task HizmetSilAsync(int id);
}
