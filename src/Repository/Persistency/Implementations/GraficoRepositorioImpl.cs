using Infrastructure.DatabaseContexts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Persistency.Abstractions;

namespace Repository.Persistency.Implementations;

public class GraficosRepositorioImpl : IGraficosRepositorio
{
    private static readonly string[] Meses = new[]
    {
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    };

    public RegisterContext Context { get; }

    public GraficosRepositorioImpl(RegisterContext context)
    {
        Context = context;
    }

    public async Task<Grafico> GetDadosGraficoByAno(Guid idUsuario, DateTime data)
    {
        int ano = data.Year;

        try
        {
            var despesas = await Context.Despesa
                .Where(d => d.UsuarioId == idUsuario && d.Data.Year == ano)
                .GroupBy(d => d.Data.Month)
                .Select(g => new { Mes = g.Key, Total = g.Sum(x => x.Valor) })
                .ToListAsync();

            var receitas = await Context.Receita
                .Where(r => r.UsuarioId == idUsuario && r.Data.Year == ano)
                .GroupBy(r => r.Data.Month)
                .Select(g => new { Mes = g.Key, Total = g.Sum(x => x.Valor) })
                .ToListAsync();


            var somatorioDespesas = Enumerable.Range(1, 12)
                .ToDictionary(
                    i => Meses[i - 1],
                    i => despesas.FirstOrDefault(x => x.Mes == i)?.Total ?? 0m
                );

            var somatorioReceitas = Enumerable.Range(1, 12)
                .ToDictionary(
                    i => Meses[i - 1],
                    i => receitas.FirstOrDefault(x => x.Mes == i)?.Total ?? 0m
                );

            return new Grafico
            {
                SomatorioDespesasPorAno = somatorioDespesas,
                SomatorioReceitasPorAno = somatorioReceitas
            };
        }
        catch
        {
            return new Grafico
            {
                SomatorioDespesasPorAno = Meses.ToDictionary(m => m, _ => 0m),
                SomatorioReceitasPorAno = Meses.ToDictionary(m => m, _ => 0m)
            };
        }
    }
}
