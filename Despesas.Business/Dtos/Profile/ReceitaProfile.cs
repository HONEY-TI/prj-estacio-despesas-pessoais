
using Despesas.Business.Dtos.Abstractions;
using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Despesas.Business.Dtos.Profile;
public class ReceitaProfile : AutoMapper.Profile
{
    public ReceitaProfile()
    {
        CreateMap<ReceitaDto, Receita>().ReverseMap();
        CreateMap<Receita, ReceitaDto>()
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.CategoriaId))
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.Categoria.Id))
            .ReverseMap();

        CreateMap<ReceitaDto, Receita>().ReverseMap();
        CreateMap<Receita, ReceitaDto>()
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.CategoriaId))
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.Categoria.Id))
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria))
            .ForPath(dest => dest.Categoria.IdTipoCategoria, opt => opt.MapFrom(src => TipoCategoria.CategoriaType.Receita))
            .ReverseMap();

        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.IdTipoCategoria, opt => opt.MapFrom(src => src.TipoCategoria.Id))
            .ReverseMap();

        CreateMap<BaseTipoCategoriaDto, TipoCategoria>().ReverseMap();
    }
}