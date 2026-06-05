using AutoMapper;
using Application.Abstractions;
using Repository.Persistency.Abstractions;

namespace Application.Implementations;
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
        var lancamentos = await  _repositorio.FindByMesAno(data, idUsuario);
        return _mapper.Map<List<Dto>>(lancamentos);
    }
}