using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Servisler;

/// <summary>
/// Hizmet yönetimi iþ kurallarýný yöneten servis.
/// </summary>
public class HizmetServisi : IHizmetServisi
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Hizmet> _hizmetDeposu;

    public HizmetServisi(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Hizmet> hizmetDeposu)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hizmetDeposu = hizmetDeposu;
    }

    public async Task<IReadOnlyList<HizmetDto>> TumHizmetleriGetirAsync()
    {
        var hizmetler = await _hizmetDeposu.TumunuGetirAsync();
        return hizmetler.Select(h => _mapper.Map<HizmetDto>(h)).ToList();
    }

    public async Task<IReadOnlyList<HizmetDto>> AktifHizmetleriGetirAsync()
    {
        var hizmetler = await _hizmetDeposu.FiltreliGetirAsync(h => h.AktifMi);
    return hizmetler.Select(h => _mapper.Map<HizmetDto>(h)).ToList();
    }

    public async Task<HizmetDto?> HizmetGetirAsync(int id)
    {
        var hizmet = await _hizmetDeposu.GetirAsync(id);
        return hizmet == null ? null : _mapper.Map<HizmetDto>(hizmet);
    }

 public async Task<HizmetDto> HizmetEkleAsync(HizmetDto dto)
    {
        var entity = _mapper.Map<Hizmet>(dto);
    await _hizmetDeposu.EkleAsync(entity);
        await _unitOfWork.KaydetAsync();
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<HizmetDto> HizmetGuncelleAsync(HizmetDto dto)
    {
        var entity = await _hizmetDeposu.GetirAsync(dto.Id)
            ?? throw new InvalidOperationException("Hizmet bulunamadý.");

  entity.Ad = dto.Ad;
        entity.Sure = dto.Sure;
        entity.Ucret = dto.Ucret;
        entity.AktifMi = dto.AktifMi;

     _hizmetDeposu.Guncelle(entity);
        await _unitOfWork.KaydetAsync();
        return dto;
    }

    public async Task HizmetSilAsync(int id)
    {
  var entity = await _hizmetDeposu.GetirAsync(id)
            ?? throw new InvalidOperationException("Hizmet bulunamadý.");

        _hizmetDeposu.Sil(entity);
        await _unitOfWork.KaydetAsync();
    }
}
