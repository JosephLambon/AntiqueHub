using AutoMapper;
using DocumentMiddleware.Core.Models;

namespace DocumentMiddleware.Api.Profiles;

public class AntiqueProfile : Profile
{
    public AntiqueProfile()
    {
        CreateMap<AntiqueForCreationDto, Antique>()
            .ForMember(dest => dest.Images,
            opt => opt.MapFrom((src, dest, destMember, context) =>
            context.Items["FileNames"]
            ));
        CreateMap<Antique, AntiqueForResponseDto>();
    }
}