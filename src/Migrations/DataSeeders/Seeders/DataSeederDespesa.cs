using Infrastructure.DatabaseContexts;
using Domain.Entities;
using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Seeders;

public class DataSeederDespesa : ISeeder
{
    private readonly RegisterContext _context;
    private readonly Random _random = new();

    public DataSeederDespesa(RegisterContext context)
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
        var user = _context.Usuario.FirstOrDefault(u => u.Nome.Equals("Teste"));
        if (user == null) return;

        var categorias = _context.Categoria
            .Where(c => c.UsuarioId == user.Id)
            .GroupBy(c => c.Descricao)
            .ToDictionary(g => g.Key, g => g.First().Id);

        var despesasBase = new List<(string Descricao, string Categoria)>
        {
            ("Conta de Luz", "Casa"),
            ("Compra de mantimentos", "Casa"),
            ("Serviço de Limpeza", "Serviços"),
            ("Consulta Médica", "Saúde"),
            ("Imposto de Renda", "Imposto"),
            ("Passagem de Ônibus", "Transporte"),
            ("Cinema", "Lazer"),
            ("Outros gastos", "Outros"),
            ("Compra de roupas", "Casa"),
            ("Serviço de manutenção", "Serviços")
        };

        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2025, 12, 31);

        var despesas = GenerateDespesas(user, categorias, despesasBase, startDate, endDate);

        foreach (var despesa in despesas)
        {
            if (isInsert)
            {
                _context.Add(despesa);
            }
            else
            {
                var existing = _context.Despesa.FirstOrDefault(d =>
                    d.Descricao == despesa.Descricao &&
                    d.UsuarioId == despesa.UsuarioId &&
                    d.Data == despesa.Data);

                if (existing != null)
                {
                    existing.Valor = despesa.Valor;
                    existing.DataVencimento = despesa.DataVencimento;
                }
                else
                {
                    _context.Add(despesa);
                }
            }
        }

        _context.SaveChanges();
    }

    private List<Despesa> GenerateDespesas(
       Usuario user,
       Dictionary<string, Guid> categorias, // <-- aqui era int
       List<(string Descricao, string Categoria)> despesasBase,
       DateTime startDate,
       DateTime endDate)
    {
        var result = new List<Despesa>();

        for (var date = startDate; date <= endDate; date = date.AddMonths(1))
        {
            foreach (var item in despesasBase)
            {
                var despesa = CreateRandomDespesa(user, categorias, item, date);
                result.Add(despesa);
            }
        }

        return result;
    }

    private Despesa CreateRandomDespesa(Usuario user,
      Dictionary<string, Guid> categorias,
      (string Descricao, string Categoria) item,
      DateTime month)
    {
        int day = _random.Next(1, DateTime.DaysInMonth(month.Year, month.Month) + 1);
        var data = new DateTime(month.Year, month.Month, day);

        var vencimento = data.AddDays(_random.Next(5, 16));
        if (vencimento.Month != month.Month)
            vencimento = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));

        decimal valor = Math.Round((decimal)(_random.NextDouble() * 1000 + 10), 2);

        Guid categoriaId;
        if (!categorias.TryGetValue(item.Categoria, out categoriaId))
        {
            if (!categorias.TryGetValue("Outros", out categoriaId))
            {
                categoriaId = categorias.Values.First();
            }
        }

        return new Despesa
        {
            Data = data,
            Descricao = item.Descricao,
            Valor = valor,
            DataVencimento = vencimento,
            Usuario = user,
            UsuarioId = user.Id,
            CategoriaId = categoriaId
        };
    }
}
