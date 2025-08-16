
using Domain.Entities;

namespace Despesas.Business.Dtos.Profile;
public class UsuarioProfile : AutoMapper.Profile
{
    public UsuarioProfile()
    {

        CreateMap<UsuarioDto, Usuario>().ReverseMap();
        CreateMap<Usuario, UsuarioDto>().ReverseMap();

        CreateMap<UsuarioDto, Usuario>().ReverseMap();
        CreateMap<Usuario, UsuarioDto>().ReverseMap();
    }
}