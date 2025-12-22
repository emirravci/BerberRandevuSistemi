using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Enumlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Servisler;

/// <summary>
/// Randevu iş kurallarını yöneten servis.
/// </summary>
public class RandevuServisi : IRandevuServisi
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RandevuServisi(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RandevuDto> RandevuOlusturAsync(RandevuDto randevuDto)
    {
        // Aynı personel aynı tarih/saatte iki randevu alamaz
        var cakisanVarMi = await _unitOfWork.RandevuDeposu
            .PersonelIcinZamanAraligindaRandevuVarMiAsync(
                randevuDto.PersonelId,
                randevuDto.Tarih,
                randevuDto.Saat);

        if (cakisanVarMi)
            throw new InvalidOperationException("Seçilen tarih ve saatte personelin başka bir randevusu bulunmaktadır.");

        // Personelin çalışma saati dışında randevu oluşturulamaz
        // (Basitleştirilmiş kontrol: İleride detaylandırılabilir)
        // Burada sadece altyapı hazırlığı yapılmış, gerçek kontrol
        // CalismaSaati tablosu ile genişletilebilir.

        var randevu = _mapper.Map<Randevu>(randevuDto);
        randevu.Durum = RandevuDurumu.Beklemede;

        await _unitOfWork.RandevuDeposu.EkleAsync(randevu);
        await _unitOfWork.KaydetAsync();

        randevuDto.Id = randevu.Id;
        randevuDto.Durum = randevu.Durum;
        return randevuDto;
    }

    public async Task RandevuDurumunuGuncelleAsync(int randevuId, RandevuDurumu yeniDurum)
    {
        var randevu = await _unitOfWork.RandevuDeposu.GetirAsync(randevuId)
                      ?? throw new InvalidOperationException("Randevu bulunamadı.");

        randevu.Durum = yeniDurum;
        _unitOfWork.RandevuDeposu.Guncelle(randevu);
        await _unitOfWork.KaydetAsync();
    }

    public async Task<IReadOnlyList<RandevuDto>> KullaniciRandevulariniGetirAsync(string musteriId)
    {
        var randevular = await _unitOfWork.RandevuDeposu
            .FiltreliGetirAsync(r => r.MusteriId == musteriId);

        return randevular
            .Select(r => _mapper.Map<RandevuDto>(r))
            .ToList();
    }

    public async Task<IReadOnlyList<RandevuDto>> PersonelRandevulariniGetirAsync(int personelId, DateTime? tarih = null)
    {
        var randevular = await _unitOfWork.RandevuDeposu
            .PersonelRandevulariniGetirAsync(personelId, tarih, tarih);

        return randevular
            .Select(r => _mapper.Map<RandevuDto>(r))
            .ToList();
    }
}


