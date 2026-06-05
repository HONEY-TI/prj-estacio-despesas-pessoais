using Infrastructure.DatabaseContexts;
using Domain.Entities;
using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Updaters;
public class DataSeederUpdateDespesa : IUpdater
{
    private readonly RegisterContext _context;
    private readonly Random _random = new Random();

    public DataSeederUpdateDespesa(RegisterContext context)
    {
        _context = context;
    }

    public void Update()
    {
        var user = _context.Usuario.FirstOrDefault(u => u.Nome.Equals("Teste"));
        if (user == null) return;

        var despesas = GerarDespesasAleatorias(user.Id, user);

        _context.Despesa.AddRange(despesas);
        _context.SaveChanges();

    }

    private List<Despesa> GerarDespesasAleatorias(Guid usuarioId, Usuario user)
    {
        var despesas = new List<Despesa>();

        for (int mes = 1; mes <= 12; mes++)
        {
            var categoriaIds = _context.Categoria
                .Where(c => c.UsuarioId == usuarioId)
                .Select(c => c.Id)
                .ToList();

            var random = new Random();
            var  randomCategoriaId = categoriaIds[random.Next(categoriaIds.Count)];
            var categoria = _context.Categoria.FirstOrDefault(c => c.Id == randomCategoriaId);
            despesas.Add(new Despesa
            {
                Data =  DateTime.SpecifyKind(new DateTime(2025, mes, 1), DateTimeKind.Utc),
                Descricao = $"Despesa mês {mes}",
                Valor = GerarValorAleatorio(50, 500), 
                DataVencimento = DateTime.SpecifyKind(new DateTime(2025, mes, 1), DateTimeKind.Utc).AddDays(30),
                Usuario = user,
                UsuarioId = usuarioId,
                Categoria = categoria,
                CategoriaId = randomCategoriaId
            });

        }

        return despesas;
    }
 
    private decimal GerarValorAleatorio(int min, int max)
    {
        return (decimal)(_random.NextDouble() * (max - min) + min);
    }
}