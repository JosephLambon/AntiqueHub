using AutoMapper;
using AntiqueHub.Core.Models;
using AntiqueHub.Core.Entities;

namespace AntiqueHub.Api.Profiles;

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