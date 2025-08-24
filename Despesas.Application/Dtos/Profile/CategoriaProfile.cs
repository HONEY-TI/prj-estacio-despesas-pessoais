using Despesas.Application.Dtos.Core;
using Domain.Core.ValueObject;
using Domain.Entities;

namespace Despesas.Application.Dtos.Profile;
public class CategoriaProfile : AutoMapper.Profile
{
    public CategoriaProfile()
    {
        CreateMap<CategoriaDto, Categoria>()
            .ForMember(dest => dest.TipoCategoria, opt => opt.MapFrom(src => src.IdTipoCategoria))
            .ReverseMap();

        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.IdTipoCategoria, opt => opt.MapFrom(src => src.TipoCategoria.Id))
            .ReverseMap();

        CreateMap<CategoriaDto, Categoria>()
            .ForMember(dest => dest.TipoCategoria, opt => opt.MapFrom(src => src.IdTipoCategoria))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ReverseMap();


        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.IdTipoCategoria, opt => opt.MapFrom(src => src.TipoCategoria.Id))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ReverseMap();

        CreateMap<TipoCategoriaDto, TipoCategoria>().ReverseMap();
    }
}