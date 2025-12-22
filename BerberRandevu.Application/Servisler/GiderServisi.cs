using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Servisler;

/// <summary>
/// Gider yönetimi ve raporlama iş kurallarını yöneten servis.
/// </summary>
public class GiderServisi : IGiderServisi
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Gider> _giderDeposu;

    public GiderServisi(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Gider> giderDeposu)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _giderDeposu = giderDeposu;
    }

    public async Task<GiderDto> GiderEkleAsync(GiderDto dto)
    {
        var gider = _mapper.Map<Gider>(dto);
        await _giderDeposu.EkleAsync(gider);
        await _unitOfWork.KaydetAsync();
        dto.Id = gider.Id;
        return dto;
    }

    public async Task<GiderDto> GiderGuncelleAsync(GiderDto dto)
    {
        var entity = await _giderDeposu.GetirAsync(dto.Id)
                     ?? throw new InvalidOperationException("Gider bulunamadı.");

        entity.Baslik = dto.Baslik;
        entity.Tutar = dto.Tutar;
        entity.Tarih = dto.Tarih;
        entity.Aciklama = dto.Aciklama;

        _giderDeposu.Guncelle(entity);
        await _unitOfWork.KaydetAsync();
        return dto;
    }

    public async Task GiderSilAsync(int id)
    {
        var entity = await _giderDeposu.GetirAsync(id)
                     ?? throw new InvalidOperationException("Gider bulunamadı.");

        _giderDeposu.Sil(entity);
        await _unitOfWork.KaydetAsync();
    }

    public async Task<IReadOnlyList<GiderDto>> TarihAraligindaGiderleriGetirAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = await _giderDeposu.FiltreliGetirAsync(g =>
            g.Tarih.Date >= baslangic.Date && g.Tarih.Date <= bitis.Date);

        return liste.Select(g => _mapper.Map<GiderDto>(g)).ToList();
    }
}


