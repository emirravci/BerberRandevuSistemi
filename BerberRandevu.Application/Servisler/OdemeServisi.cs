using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Enumlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Servisler;

/// <summary>
/// Ödeme alma ve raporlama iş kurallarını yöneten servis.
/// </summary>
public class OdemeServisi : IOdemeServisi
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Odeme> _odemeDeposu;

    public OdemeServisi(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Odeme> odemeDeposu)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _odemeDeposu = odemeDeposu;
    }

    public async Task<OdemeDto> OdemeAlAsync(OdemeDto dto)
    {
        var randevu = await _unitOfWork.RandevuDeposu.GetirAsync(dto.RandevuId)
                      ?? throw new InvalidOperationException("Randevu bulunamadı.");

        if (randevu.Durum != RandevuDurumu.Onaylandi &&
            randevu.Durum != RandevuDurumu.Tamamlandi)
        {
            throw new InvalidOperationException("Sadece onaylanmış veya tamamlanmış randevular için ödeme alınabilir.");
        }

        var odeme = _mapper.Map<Odeme>(dto);
        odeme.OdemeTarihi = dto.OdemeTarihi == default ? DateTime.UtcNow : dto.OdemeTarihi;

        randevu.OdemeAlindiMi = true;

        await _odemeDeposu.EkleAsync(odeme);
        _unitOfWork.RandevuDeposu.Guncelle(randevu);
        await _unitOfWork.KaydetAsync();
        return _mapper.Map<OdemeDto>(odeme);
    }

    public async Task<IReadOnlyList<OdemeDto>> TarihAraligindaOdemeleriGetirAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = await _odemeDeposu.FiltreliGetirAsync(o =>
            o.OdemeTarihi.Date >= baslangic.Date && o.OdemeTarihi.Date <= bitis.Date);

        return liste.Select(o => _mapper.Map<OdemeDto>(o)).ToList();
    }
}


