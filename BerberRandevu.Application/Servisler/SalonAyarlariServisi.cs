using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Servisler;

/// <summary>
/// Salon ayarlarý yönetimi servis implementasyonu.
/// </summary>
public class SalonAyarlariServisi : ISalonAyarlariServisi
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
 private readonly IGenericRepository<SalonAyarlari> _ayarlarDeposu;

    // Ayar anahtarlarý
    private const string CALISMA_SAAT_BASLANGIC = "CalismaSaatBaslangic";
    private const string CALISMA_SAAT_BITIS = "CalismaSaatBitis";
    private const string RANDEVU_DILIMI = "RandevuDilimiDakika";

    public SalonAyarlariServisi(
        IUnitOfWork unitOfWork,
  IMapper mapper,
      IGenericRepository<SalonAyarlari> ayarlarDeposu)
    {
        _unitOfWork = unitOfWork;
   _mapper = mapper;
        _ayarlarDeposu = ayarlarDeposu;
    }

  public async Task<CalismaSaatleriAyarDto> CalismaSaatleriAyarlariniGetirAsync()
    {
        var baslangic = await AyarGetirAsync(CALISMA_SAAT_BASLANGIC);
        var bitis = await AyarGetirAsync(CALISMA_SAAT_BITIS);
        var dilim = await AyarGetirAsync(RANDEVU_DILIMI);

        return new CalismaSaatleriAyarDto
        {
 BaslangicSaati = string.IsNullOrEmpty(baslangic) 
? new TimeSpan(9, 0, 0) 
     : TimeSpan.Parse(baslangic),
    BitisSaati = string.IsNullOrEmpty(bitis) 
  ? new TimeSpan(20, 0, 0) 
     : TimeSpan.Parse(bitis),
          RandevuDilimiDakika = string.IsNullOrEmpty(dilim) 
        ? 30 
         : int.Parse(dilim)
        };
    }

    public async Task CalismaSaatleriAyarlariniKaydetAsync(CalismaSaatleriAyarDto dto)
    {
        await AyarKaydetAsync(CALISMA_SAAT_BASLANGIC, dto.BaslangicSaati.ToString(@"hh\:mm"), "Çalýþma saati baþlangýcý");
 await AyarKaydetAsync(CALISMA_SAAT_BITIS, dto.BitisSaati.ToString(@"hh\:mm"), "Çalýþma saati bitiþi");
        await AyarKaydetAsync(RANDEVU_DILIMI, dto.RandevuDilimiDakika.ToString(), "Randevu zaman dilimi (dakika)");
    }

    public async Task<string?> AyarGetirAsync(string anahtar)
    {
  var ayarlar = await _ayarlarDeposu.FiltreliGetirAsync(a => a.Anahtar == anahtar);
  return ayarlar.FirstOrDefault()?.Deger;
    }

    public async Task AyarKaydetAsync(string anahtar, string deger, string? aciklama = null)
    {
  var mevcutAyarlar = await _ayarlarDeposu.FiltreliGetirAsync(a => a.Anahtar == anahtar);
        var mevcutAyar = mevcutAyarlar.FirstOrDefault();

        if (mevcutAyar != null)
        {
       mevcutAyar.Deger = deger;
        if (aciklama != null)
  mevcutAyar.Aciklama = aciklama;
        _ayarlarDeposu.Guncelle(mevcutAyar);
   }
        else
        {
    var yeniAyar = new SalonAyarlari
{
         Anahtar = anahtar,
                Deger = deger,
           Aciklama = aciklama
 };
      await _ayarlarDeposu.EkleAsync(yeniAyar);
        }

        await _unitOfWork.KaydetAsync();
    }
}
