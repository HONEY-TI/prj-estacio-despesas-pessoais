using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Implementations;

public class DataSeeder : IDataSeeder
{
    private readonly IDataSeederAcesso _acessoSeeder;
    private readonly IDataSeederDespesa _despesaSeeder;
    private readonly IDataSeederReceita _receitaSeeder;
    private readonly IDataSeederUpdateDespesa _updateDespesaSeeder;
    private readonly IDataSeederUpdateReceita _updateReceitaSeeder;

    public DataSeeder(
        IDataSeederAcesso acessoSeeder,
        IDataSeederDespesa despesaSeeder,
        IDataSeederReceita receitaSeeder,
        IDataSeederUpdateDespesa updateDespesaSeeder,
        IDataSeederUpdateReceita updateReceitaSeeder)
    {
        _acessoSeeder = acessoSeeder;
        _despesaSeeder = despesaSeeder;
        _receitaSeeder = receitaSeeder;
        _updateDespesaSeeder = updateDespesaSeeder;
        _updateReceitaSeeder = updateReceitaSeeder;
    }

    public void Insert()
    {
        _acessoSeeder.Insert();
        _despesaSeeder.Insert();
        _receitaSeeder.Insert();
    }

    public void Update()
    {
        _updateDespesaSeeder.Update();
        _updateReceitaSeeder.Update();
    }
}
