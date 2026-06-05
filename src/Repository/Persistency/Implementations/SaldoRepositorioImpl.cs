using System.Data;
using Infrastructure.DatabaseContexts;
using Repository.Persistency.Abstractions;

namespace Repository.Persistency.Implementations;
public class SaldoRepositorioImpl : ISaldoRepositorio
{
    public RegisterContext Context { get; }
    public SaldoRepositorioImpl(RegisterContext context)
    {
        Context = context;
    }

    public decimal GetSaldo(Guid idUsuario)
    {
        decimal sumDespesa = Context.Despesa.Where(d => d.UsuarioId == idUsuario).AsEnumerable().Sum(d => d.Valor);
        decimal sumReceita = Context.Receita.Where(r => r.UsuarioId == idUsuario).AsEnumerable().Sum(r => r.Valor);
        return (sumReceita - sumDespesa);
    }

    public decimal GetSaldoByAno(DateTime mesAno, Guid idUsuario)
    {
        int ano = mesAno.Year;
        decimal sumDespesa = Context.Despesa.Where(d => d.UsuarioId == idUsuario && d.Data.Year == ano).AsEnumerable().Sum(d => d.Valor);
        decimal sumReceita = Context.Receita.Where(r => r.UsuarioId == idUsuario && r.Data.Year == ano).AsEnumerable().Sum(r => r.Valor);
        return (sumReceita - sumDespesa);
    }

    public decimal GetSaldoByMesAno(DateTime mesAno, Guid idUsuario)
    {
        int mes = mesAno.Month;
        int ano = mesAno.Year;
        decimal sumDespesa = Context.Despesa.Where(d => d.UsuarioId == idUsuario && d.Data.Year == ano && d.Data.Month == mes).AsEnumerable().Sum(d => d.Valor);
        decimal sumReceita = Context.Receita.Where(r => r.UsuarioId == idUsuario && r.Data.Year == ano && r.Data.Month == mes).AsEnumerable().Sum(r => r.Valor);
        return (sumReceita - sumDespesa);
    }
}

