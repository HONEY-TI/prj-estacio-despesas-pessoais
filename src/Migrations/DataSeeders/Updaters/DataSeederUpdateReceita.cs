using Infrastructure.DatabaseContexts;
using Domain.Entities;
using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Updaters;
public class DataSeederUpdateReceita : IUpdater
{
    private readonly RegisterContext _context;
    private readonly Random _random = new Random();

    public DataSeederUpdateReceita(RegisterContext context)
    {
        _context = context;
    }

    public void Update()
    {
        var user = _context.Usuario.FirstOrDefault(u => u.Nome.Equals("Teste"));
        if (user == null) return;

        var receitas = GerarReceitasAleatorias(user.Id);

        _context.Receita.AddRange(receitas);
        _context.SaveChanges();

    }

    private List<Receita> GerarReceitasAleatorias(Guid usuarioId)
    {
        var receitas = new List<Receita>();

        for (int mes = 1; mes <= 12; mes++)
        {
            var categoriaIds = _context.Categoria
                     .Where(c => c.UsuarioId == usuarioId)
                     .Select(c => c.Id)
                     .ToList();

            var random = new Random();
            var randomCategoriaId = categoriaIds[random.Next(categoriaIds.Count)];
            var categoria = _context.Categoria.FirstOrDefault(c => c.Id == randomCategoriaId);            
            receitas.Add(new Receita
            {
                Data = DateTime.SpecifyKind(new DateTime(2025, mes, 1), DateTimeKind.Utc),
                Descricao = $"Receita mês {mes}",
                Valor = GerarValorAleatorio(50, 500),
                UsuarioId = usuarioId,
                Categoria = categoria,
                CategoriaId = randomCategoriaId
            });


        }

        return receitas;
    }

    private decimal GerarValorAleatorio(int min, int max)
    {
        return (decimal)(_random.NextDouble() * (max - min) + min);
    }
}