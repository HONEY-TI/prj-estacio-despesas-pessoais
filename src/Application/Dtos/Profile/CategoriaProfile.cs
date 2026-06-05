using Application.Dtos;
using Application.Dtos.Core;
using Domain.Core.ValueObject;
using Domain.Entities;

public class CategoriaProfile : AutoMapper.Profile
{
    public CategoriaProfile()
    {
        // DTO -> Entidade
        CreateMap<CategoriaDto, Categoria>()
            .ForMember(dest => dest.TipoCategoriaId, opt => opt.MapFrom(src => (int)src.IdTipoCategoria))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        // Entity <-> Dto para Entidade
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.IdTipoCategoria, opt => opt.MapFrom(src => src.TipoCategoriaId))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));


        CreateMap<TipoCategoria, TipoCategoriaDto>()
            .ConvertUsing(src => (TipoCategoriaDto)src.Id);
    }
}
