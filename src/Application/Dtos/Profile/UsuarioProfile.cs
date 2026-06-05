using Domain.Core.ValueObject;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Dtos.Profile;
public class UsuarioProfile : AutoMapper.Profile
{
    public UsuarioProfile()
    {
        CreateMap<UsuarioDto, Usuario>()
            .ForMember(dest => dest.PerfilUsuario,
                opt => opt.MapFrom(src => src.PerfilUsuario.HasValue
                    ? new PerfilUsuario((PerfilUsuario.Perfil)src.PerfilUsuario.Value)
                    : new PerfilUsuario(PerfilUsuario.Perfil.User)))
            .ForMember(dest => dest.Profile,
                opt => opt.MapFrom(src => src.Profile != null ? FormFileToByteArray(src.Profile) : null));

        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.PerfilUsuario,
                opt => opt.MapFrom(src => (src.PerfilUsuario == null || src.PerfilUsuario.Id == 0)
                    ? 2
                    : src.PerfilUsuario.Id))
            .ForMember(dest => dest.Profile, opt => opt.Ignore());
    }

    private static byte[]? FormFileToByteArray(IFormFile? file)
    {
        if (file == null) return null;

        using var ms = new MemoryStream();
        file.CopyTo(ms);
        return ms.ToArray();
    }
}