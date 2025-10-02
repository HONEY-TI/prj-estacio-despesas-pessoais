using Microsoft.Extensions.DependencyInjection;
using Migrations.DataSeeders.Abstractions;
using Migrations.DataSeeders.DatabaseMaintenance;
using Migrations.DataSeeders.Implementations;
using Migrations.DataSeeders.Seeders;
using Migrations.DataSeeders.Updaters;

namespace Migrations.DataSeeders.CommonDependenceInject;
public static class DataSeedersDependenceInject
{
    public static void AddDataSeeders(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<ISeeder, DataSeederAcesso>();
        services.AddScoped<ISeeder, DataSeederDespesa>();
        services.AddScoped<IUpdater, DataSeederUpdateDespesa>();
        services.AddScoped<ISeeder, DataSeederReceita>();
        services.AddScoped<IUpdater, DataSeederUpdateReceita>();
        services.AddScoped<IDatabaseMaintenance, MySqlDatabaseMaintenance>();
        services.AddScoped<IDatabaseMaintenance, SqlServerDatabaseMaintenance>();
        services.AddScoped<IDatabaseMaintenance, OracleDatabaseMaintenance>();
        services.AddScoped<IDataSeeder, DataSeeder>();

    }

    public static void RunDataSeeders(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        dataSeeder.Insert();
        dataSeeder.Update();
    }

    public static void BackupDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        dataSeeder.BackupDatabase();
    }

    public static void RestoreDatabase(this IServiceProvider serviceProvider, string file)
    {
        using var scope = serviceProvider.CreateScope();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        dataSeeder.RestoreDatabase(file);
    }
}