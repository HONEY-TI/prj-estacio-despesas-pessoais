using Domain.Entities;

namespace Despesas.Application.Dtos.Profile;
public class AcessoProfile : AutoMapper.Profile
{
    public AcessoProfile()
    {
        CreateMap<AcessoDto, Usuario>().ReverseMap();
        CreateMap<AcessoDto, GoogleAuthenticationDto>().ReverseMap();

        CreateMap<Acesso, AcessoDto>().AfterMap((s, d) =>
        {
            d.Senha = "********";
        }).ReverseMap();


        CreateMap<AcessoDto, Usuario>().ReverseMap();
        CreateMap<Acesso, AcessoDto>().AfterMap((s, d) =>
        {
            d.Senha = "********";
        }).ReverseMap();
              

    }
}