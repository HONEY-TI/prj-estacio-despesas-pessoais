using Business.Abstractions.Generic;
using Domain.Entities;

namespace Business.Dtos.Core.Profile;
public class ControleAcessoProfile : AutoMapper.Profile
{
    public ControleAcessoProfile()
    {
        CreateMap<Business.Dtos.v1.ControleAcessoDto, Usuario>().ReverseMap();
        CreateMap<ControleAcesso, Business.Dtos.v1.ControleAcessoDto>().AfterMap((s, d) =>
        {
            d.Senha = "********";
        }).ReverseMap();


        CreateMap<Business.Dtos.v2.ControleAcessoDto, Usuario>().ReverseMap();
        CreateMap<ControleAcesso, Business.Dtos.v2.ControleAcessoDto>().AfterMap((s, d) =>
        {
            d.Senha = "********";
        }).ReverseMap();


        CreateMap<Business.Dtos.v2.ControleAcessoDto,Business.Dtos.v2.LoginDto>()
           .ForMember(dest => dest.Senha, opt => opt.MapFrom(src => src.Senha ?? Guid.NewGuid().ToString("N")))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.ExternalProvider, opt => opt.MapFrom(src => src.ExternalProvider))
           .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId)).ReverseMap();

    }
}