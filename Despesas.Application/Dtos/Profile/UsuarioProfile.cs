using Domain.Core.ValueObject;
using Domain.Entities;

namespace Despesas.Application.Dtos.Profile;
public class UsuarioProfile : AutoMapper.Profile
{
    public UsuarioProfile()
    {
        CreateMap<UsuarioDto, Usuario>()
            .ForMember(dest => dest.PerfilUsuario,
            opt => opt.MapFrom(src => src.PerfilUsuario.HasValue
            ? new PerfilUsuario((PerfilUsuario.Perfil)src.PerfilUsuario.Value)
            : new PerfilUsuario(PerfilUsuario.Perfil.User)));

        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.PerfilUsuario,
            opt => opt.MapFrom(src => (src.PerfilUsuario == null || src.PerfilUsuario.Id == 0)
            ? 2
            : src.PerfilUsuario.Id));


    }
}