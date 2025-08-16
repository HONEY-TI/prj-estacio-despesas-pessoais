using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Repository.Persistency.Abstractions;

namespace Despesas.Application.Implementations;
public class SaldoBusinessImpl : ISaldoBusiness
{
    private readonly ISaldoRepositorio _repositorio;

    public SaldoBusinessImpl(ISaldoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public SaldoDto GetSaldo(Guid idUsuario)
    {
        return _repositorio.GetSaldo(idUsuario);
    }

    public SaldoDto GetSaldoAnual(DateTime ano, Guid idUsuario)
    {
        return _repositorio.GetSaldoByAno(ano, idUsuario);
    }

    public SaldoDto GetSaldoByMesAno(DateTime mesAno, Guid idUsuario)
    {
        return _repositorio.GetSaldoByMesAno(mesAno, idUsuario);
    }   
}
