using Application.Dtos.Core;
using Domain.Core.ValueObject;
using Domain.Entities;

namespace Application.Dtos.Profile;
public class ReceitaProfile : AutoMapper.Profile
{
    public ReceitaProfile()
    {
        // DTO <-> Entidade para Receita
        CreateMap<ReceitaDto, Receita>()
            .ForPath(dest => dest.Categoria.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
            .ForPath(dest => dest.Categoria.TipoCategoriaId, opt => opt.MapFrom(src => src.Categoria.IdTipoCategoria))
            .ReverseMap();

        // Entity <-> Dto para Entidade
        CreateMap<Receita, ReceitaDto>();

        // Entity <-> Dto para Entidade
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.IdTipoCategoria, opt => opt.MapFrom(src => src.TipoCategoriaId))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ReverseMap();


        CreateMap<TipoCategoria, TipoCategoriaDto>()
            .ConvertUsing(src => (TipoCategoriaDto)src.Id);
    }
}
