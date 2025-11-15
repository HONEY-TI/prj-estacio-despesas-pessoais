using Infrastructure.DatabaseContexts;
using Repository.Mapping.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Migrations.MySqlServer.CommonInjectDependence;
public static class MigrationsMySqlServerInjectDependence
{
    public static IServiceCollection ConfigureMySqlServerMigrationsContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("MySqlConnectionString")
           ?? configuration.GetConnectionString("SqlConnectionString") 
           ?? throw new Exception("Connection string 'SqlConnectionString' não encontrada no appsettings.json.");

        services.AddDbContext<RegisterContext>((sp, options) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            options.UseLoggerFactory(loggerFactory);
            options.UseLazyLoadingProxies();
        });

        var provider = DatabaseProvider.MySql;
        services.AddSingleton(typeof(DatabaseProvider), provider);

        services.AddScoped<RegisterContext>(sp =>
        {
            var options = sp.GetRequiredService<DbContextOptions<RegisterContext>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new RegisterContext(options, provider, loggerFactory);
        });

        return services;
    }
}