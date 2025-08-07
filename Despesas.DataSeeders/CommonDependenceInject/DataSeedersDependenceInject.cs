using DataSeeders;
using DataSeeders.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Despesas.DataSeeders.CommonDependenceInject;
public static class DataSeedersDependenceInject
{
    public static void AddDataSeeders(this IServiceCollection services)
    {
        services.AddTransient<IDataSeeder, DataSeeder>();
    }

    public static void RunDataSeeders(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        dataSeeder.SeedData();
    }
}