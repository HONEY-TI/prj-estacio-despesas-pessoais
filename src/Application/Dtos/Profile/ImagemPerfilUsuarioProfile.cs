using Domain.Entities;

namespace Application.Dtos.Profile;

public class ImagemPerfilUsuarioProfile : AutoMapper.Profile
{
    public ImagemPerfilUsuarioProfile()
    {
        CreateMap<ImagemPerfilDto, ImagemPerfilUsuario>().ReverseMap();
        CreateMap<ImagemPerfilUsuario, ImagemPerfilDto>().ReverseMap();

        CreateMap<ImagemPerfilDto, ImagemPerfilUsuario>().ReverseMap();
        CreateMap<ImagemPerfilUsuario, ImagemPerfilDto>().ReverseMap();

        CreateMap<UsuarioDto, Usuario>().ReverseMap();
        CreateMap<UsuarioDto, Usuario>().ReverseMap();
    }
}