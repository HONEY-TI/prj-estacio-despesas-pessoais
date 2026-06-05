using Application.Abstractions;
using Application.Dtos;
using Repository.Persistency.Abstractions;

namespace Application.Implementations;
public class SaldoBusinessImpl : ISaldoBusiness
{
    private readonly ISaldoRepositorio _repositorio;

    public SaldoBusinessImpl(ISaldoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<SaldoDto> GetSaldo(Guid idUsuario)
    {
        return await Task.FromResult(_repositorio.GetSaldo(idUsuario));
    }

    public async Task<SaldoDto> GetSaldoAnual(DateTime ano, Guid idUsuario)
    {
        return await Task.FromResult(_repositorio.GetSaldoByAno(ano, idUsuario));
    }

    public async Task<SaldoDto> GetSaldoByMesAno(DateTime mesAno, Guid idUsuario)
    {
        return await Task.FromResult(_repositorio.GetSaldoByMesAno(mesAno, idUsuario));
    }   
}
