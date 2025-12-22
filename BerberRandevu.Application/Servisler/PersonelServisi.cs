using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Servisler;

/// <summary>
/// Personel yönetimi iş kurallarını yöneten servis.
/// </summary>
public class PersonelServisi : IPersonelServisi
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PersonelServisi(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PersonelDto>> TumPersonelleriGetirAsync()
    {
        var personeller = await _unitOfWork.PersonelDeposu.TumunuGetirAsync();
        return personeller.Select(p => _mapper.Map<PersonelDto>(p)).ToList();
    }

    public async Task<IReadOnlyList<PersonelDto>> AktifPersonelleriGetirAsync()
    {
        var personeller = await _unitOfWork.PersonelDeposu.AktifPersonelleriGetirAsync();
        return personeller.Select(p => _mapper.Map<PersonelDto>(p)).ToList();
    }

    public async Task<PersonelDto?> PersonelGetirAsync(int id)
    {
        var personel = await _unitOfWork.PersonelDeposu.GetirAsync(id);
        return personel == null ? null : _mapper.Map<PersonelDto>(personel);
    }

    public async Task<PersonelDto> PersonelEkleAsync(PersonelDto dto)
    {
        var entity = _mapper.Map<Personel>(dto);
        await _unitOfWork.PersonelDeposu.EkleAsync(entity);
        await _unitOfWork.KaydetAsync();
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<PersonelDto> PersonelGuncelleAsync(PersonelDto dto)
    {
        var entity = await _unitOfWork.PersonelDeposu.GetirAsync(dto.Id)
                     ?? throw new InvalidOperationException("Personel bulunamadı.");

        entity.Ad = dto.Ad;
        entity.Soyad = dto.Soyad;
        entity.AktifMi = dto.AktifMi;

        _unitOfWork.PersonelDeposu.Guncelle(entity);
        await _unitOfWork.KaydetAsync();
        return dto;
    }

    public async Task PersonelSilAsync(int id)
    {
        var entity = await _unitOfWork.PersonelDeposu.GetirAsync(id)
                     ?? throw new InvalidOperationException("Personel bulunamadı.");

        _unitOfWork.PersonelDeposu.Sil(entity);
        await _unitOfWork.KaydetAsync();
    }
}


