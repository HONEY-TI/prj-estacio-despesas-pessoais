using Infrastructure.DatabaseContexts;
using Repository.Mapping.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure.CommonInjectDependence;

public static class SqlServicesInjectDependence
{

    public static IServiceCollection ConfigureMsSqlServerContext(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = DatabaseProvider.SqlServer;
        services.AddSingleton(typeof(DatabaseProvider), provider);

        services.AddScoped<RegisterContext>(sp =>
        {
            var options = sp.GetRequiredService<DbContextOptions<RegisterContext>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new RegisterContext(options, provider, loggerFactory);
        });

        string connectionString = configuration.GetConnectionString("SqlServerConnectionString")
           ?? configuration.GetConnectionString("SqlConnectionString")
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

    public static IServiceCollection ConfigureMySqlServerContext(this IServiceCollection services, IConfiguration configuration)
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
                    b => b.MigrationsAssembly("Migrations.MySqlServer"));
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

    public static IServiceCollection ConfigureOracleServerContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("OracleConnectionString")
           ?? configuration.GetConnectionString("SqlConnectionString")
           ?? throw new Exception("Connection string 'SqlConnectionString' não encontrada no appsettings.json.");

        services.AddDbContext<RegisterContext>((sp, options) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseOracle(
                connectionString,
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            options.UseLoggerFactory(loggerFactory);
            options.UseLazyLoadingProxies();
        });

        var provider = DatabaseProvider.Oracle;
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