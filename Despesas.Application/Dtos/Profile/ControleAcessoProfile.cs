using Domain.Entities;

namespace Despesas.Application.Dtos.Profile;
public class ControleAcessoProfile : AutoMapper.Profile
{
    public ControleAcessoProfile()
    {
        CreateMap<ControleAcessoDto, Usuario>().ReverseMap();
        CreateMap<ControleAcessoDto, GoogleAuthenticationDto>().ReverseMap();

        CreateMap<ControleAcesso, ControleAcessoDto>().AfterMap((s, d) =>
        {
            d.Senha = "********";
        }).ReverseMap();


        CreateMap<ControleAcessoDto, Usuario>().ReverseMap();
        CreateMap<ControleAcesso, ControleAcessoDto>().AfterMap((s, d) =>
        {
            d.Senha = "********";
        }).ReverseMap();
              

    }
}