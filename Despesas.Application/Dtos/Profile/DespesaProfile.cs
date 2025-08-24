using Despesas.Application.Dtos.Core;
using Domain.Core.ValueObject;
using Domain.Entities;

namespace Despesas.Application.Dtos.Profile;
public class DespesaProfile : AutoMapper.Profile
{
    public DespesaProfile()
    {
        CreateMap<DespesaDto, Despesa>().ReverseMap();

        CreateMap<Despesa, DespesaDto>()
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.CategoriaId))
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.Categoria.Id))
            .ReverseMap();


        CreateMap<DespesaDto, Despesa>()
            .ReverseMap();

        CreateMap<Despesa, DespesaDto>()
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.CategoriaId))
            .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.Categoria.Id))
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria))
            .ForPath(dest => dest.Categoria.IdTipoCategoria, opt => opt.MapFrom(src => TipoCategoria.CategoriaType.Despesa))
            .ReverseMap();


        CreateMap<Categoria, CategoriaDto>()
        .ForMember(dest => dest.IdTipoCategoria, opt => opt.MapFrom(src => src.TipoCategoria.Id))
        .ReverseMap();

        CreateMap<TipoCategoriaDto, TipoCategoria>().ReverseMap();
    }
}