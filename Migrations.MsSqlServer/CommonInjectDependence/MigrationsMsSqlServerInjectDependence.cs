using Despesas.Infrastructure.DatabaseContexts;
using Despesas.Repository.Mapping.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Migrations.MsSqlServer.CommonInjectDependence;

public static class MigrationsMsSqlServerInjectDependence
{
    public static IServiceCollection ConfigureMsSqlServerMigrationsContext(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = DatabaseProvider.SqlServer;
        services.AddSingleton(typeof(DatabaseProvider), provider);

        services.AddScoped<RegisterContext>(sp =>
        {
            var options = sp.GetRequiredService<DbContextOptions<RegisterContext>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new RegisterContext(options, provider, loggerFactory);
        });

        string connectionString = configuration.GetConnectionString("SqlConnectionString")
           ?? throw new Exception("Connection string 'SqlConnectionString' não encontrada no appsettings.json.");

        services.AddDbContext<RegisterContext>((sp, options) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            options.UseLoggerFactory(loggerFactory);
            options.UseLazyLoadingProxies();
        });  

        return services;
    }
}