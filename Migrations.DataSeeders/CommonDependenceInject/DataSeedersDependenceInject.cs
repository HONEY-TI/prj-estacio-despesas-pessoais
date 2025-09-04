using Microsoft.Extensions.DependencyInjection;
using Migrations.DataSeeders.Abstractions;
using Migrations.DataSeeders.Implementations;

namespace Migrations.DataSeeders.CommonDependenceInject;
public static class DataSeedersDependenceInject
{
    public static void AddDataSeeders(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<IDataSeederAcesso, DataSeederAcesso>();
        services.AddScoped<IDataSeederDespesa, DataSeederDespesa>();
        services.AddScoped<IDataSeederReceita, DataSeederReceita>();
        services.AddScoped<IDataSeederUpdateDespesa, DataSeederUpdateDespesa>();
        services.AddScoped<IDataSeederUpdateReceita, DataSeederUpdateReceita>();
    }

    public static void RunDataSeeders(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        dataSeeder.Insert();
        dataSeeder.Update();
    }
}