using AutoMapper;
using Despesas.Application.Abstractions;
using Repository.Persistency.Abstractions;

namespace Despesas.Application.Implementations;
public class LancamentoBusinessImpl<Dto> : ILancamentoBusiness<Dto> where Dto : class, new()
{
    private readonly ILancamentoRepositorio _repositorio;
    private readonly IMapper _mapper;
    public LancamentoBusinessImpl(IMapper mapper, ILancamentoRepositorio repositorio)
    {
        _mapper = mapper;
        _repositorio = repositorio;

    }

    public async Task<List<Dto>> FindByMesAno(DateTime data, Guid idUsuario)
    {
        return await Task.FromResult(_mapper.Map<List<Dto>>(_repositorio.FindByMesAno(data, idUsuario)));
    }
}