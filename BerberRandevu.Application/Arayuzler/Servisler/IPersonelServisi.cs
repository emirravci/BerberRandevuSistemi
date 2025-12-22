using BerberRandevu.Application.DTOlar;

namespace BerberRandevu.Application.Arayuzler.Servisler;

/// <summary>
/// Personel yönetimi için servis sözleşmesi.
/// </summary>
public interface IPersonelServisi
{
    Task<IReadOnlyList<PersonelDto>> TumPersonelleriGetirAsync();

    Task<IReadOnlyList<PersonelDto>> AktifPersonelleriGetirAsync();

    Task<PersonelDto?> PersonelGetirAsync(int id);

    Task<PersonelDto> PersonelEkleAsync(PersonelDto dto);

    Task<PersonelDto> PersonelGuncelleAsync(PersonelDto dto);

    Task PersonelSilAsync(int id);
}


