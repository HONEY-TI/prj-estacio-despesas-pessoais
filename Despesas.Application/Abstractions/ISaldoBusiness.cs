using Despesas.Application.Dtos;

namespace Despesas.Application.Abstractions;
public interface ISaldoBusiness
{
    SaldoDto GetSaldo(Guid idUsuario);
    SaldoDto GetSaldoAnual(DateTime ano, Guid idUsuario);
    SaldoDto GetSaldoByMesAno(DateTime mesAno, Guid idUsuario);
}
