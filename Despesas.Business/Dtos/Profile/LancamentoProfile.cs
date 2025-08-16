
using Domain.Entities;

namespace Despesas.Business.Dtos.Profile;
public class LancamentoProfile : AutoMapper.Profile
{
    public LancamentoProfile()
    {

        CreateMap<Lancamento, LancamentoDto>()
            .ForMember(dest => dest.IdDespesa, opt => opt.MapFrom(src => src.DespesaId))
            .ForMember(dest => dest.IdReceita, opt => opt.MapFrom(src => src.ReceitaId))
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria.Descricao))
            .ForMember(dest => dest.TipoCategoria, opt => opt.MapFrom(src => src.Categoria.TipoCategoria.Name))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.ReceitaId == Guid.Empty ? src.Despesa.Descricao : src.Receita.Descricao))
            .ReverseMap();

        CreateMap<Lancamento, LancamentoDto>()
            .ForMember(dest => dest.IdDespesa, opt => opt.MapFrom(src => src.DespesaId))
            .ForMember(dest => dest.IdReceita, opt => opt.MapFrom(src => src.ReceitaId))
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria.Descricao))
            .ForMember(dest => dest.TipoCategoria, opt => opt.MapFrom(src => src.Categoria.TipoCategoria.Name))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.ReceitaId == Guid.Empty ? src.Despesa.Descricao : src.Receita.Descricao))
            .ReverseMap();

    }
}