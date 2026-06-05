using Infrastructure.DatabaseContexts;
using Domain.Entities;
using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Seeders;

public class DataSeederReceita : ISeeder
{
    private readonly RegisterContext _context;
    private readonly Random _random = new();

    public DataSeederReceita(RegisterContext context)
    {
        _context = context;
    }

    public void Insert()
    {
        SeedInternal(isInsert: true);
    }

    public void Seed()
    {
        SeedInternal(isInsert: false);
    }

    private void SeedInternal(bool isInsert)
    {
        var user = _context.Usuario.FirstOrDefault(u => u.Nome.Contains("Teste"));
        if (user == null) return;

        var categorias = _context.Categoria
             .Where(c => c.UsuarioId == user.Id)
             .GroupBy(c => c.Descricao)
             .ToDictionary(g => g.Key, g => g.First().Id);

        var receitasBase = new List<(string Descricao, string Categoria, decimal Valor)>
        {
            ("Salário", "Salário", 2000m),
            ("Investimento Bitcoin", "Investimento", 1000m),
            ("Benefício casa alugada", "Benefício", 300m),
            ("Outros ganhos", "Outros", 150m),
            ("Freelance", "Outros", 500m)
        };

        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2025, 12, 31);

        var receitas = GenerateReceitas(user, categorias, receitasBase, startDate, endDate);

        foreach (var receita in receitas)
        {
            if (isInsert)
            {
                _context.Add(receita);
            }
            else
            {
                var existing = _context.Receita.FirstOrDefault(r =>
                    r.Descricao == receita.Descricao &&
                    r.UsuarioId == receita.UsuarioId &&
                    r.Data == receita.Data);

                if (existing != null)
                {
                    existing.Valor = receita.Valor;
                }
                else
                {
                    _context.Add(receita);
                }
            }
        }

        _context.SaveChanges();
    }

    private List<Receita> GenerateReceitas(
        Usuario user,
        Dictionary<string, Guid> categorias,
        List<(string Descricao, string Categoria, decimal Valor)> receitasBase,
        DateTime startDate,
        DateTime endDate)
    {
        var result = new List<Receita>();

        for (var date = startDate; date <= endDate; date = date.AddMonths(1))
        {
            foreach (var item in receitasBase)
            {
                var receita = CreateRandomReceita(user, categorias, item, date);
                result.Add(receita);
            }
        }

        return result;
    }

    private Receita CreateRandomReceita(
        Usuario user,
        Dictionary<string, Guid> categorias,
        (string Descricao, string Categoria, decimal Valor) item,
        DateTime month)
    {
        int day = _random.Next(1, DateTime.DaysInMonth(month.Year, month.Month) + 1);
        var data = new DateTime(month.Year, month.Month, day);

        decimal valor = Math.Round(item.Valor * (decimal)(0.9 + _random.NextDouble() * 0.2), 2); // ±10% aleatório

        Guid categoriaId;
        if (!categorias.TryGetValue(item.Categoria, out categoriaId))
        {
            if (!categorias.TryGetValue("Outros", out categoriaId))
            {
                categoriaId = categorias.Values.First();
            }
        }

        return new Receita
        {
            Data = data,
            Descricao = $"{item.Descricao} - {month:MMMM/yyyy}",
            Valor = valor,
            Usuario = user,
            UsuarioId = user.Id,
            CategoriaId = categoriaId
        };
    }
}
