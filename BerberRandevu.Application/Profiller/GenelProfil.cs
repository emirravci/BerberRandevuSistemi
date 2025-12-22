using AutoMapper;
using BerberRandevu.Application.DTOlar;
using BerberRandevu.Domain.Varliklar;

namespace BerberRandevu.Application.Profiller;

/// <summary>
/// Entity-DTO dönüşümleri için AutoMapper profili.
/// </summary>
public class GenelProfil : Profile
{
    public GenelProfil()
    {
        CreateMap<Randevu, RandevuDto>()
            .ForMember(d => d.PersonelAdSoyad,
                opt => opt.MapFrom(s => s.Personel.Ad + " " + s.Personel.Soyad))
            .ReverseMap()
            .ForMember(d => d.Personel, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<Personel, PersonelDto>().ReverseMap();
        CreateMap<Odeme, OdemeDto>().ReverseMap();
        CreateMap<Gider, GiderDto>().ReverseMap();
    }
}


