using Despesas.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Migrations.Oracle.CommonInjectDependence;
public static class MigrationsOracleServerInjectDependence
{
    public static IServiceCollection ConfigureOracleServerMigrationsContext(this IServiceCollection services, IConfiguration configuration)
    {
        var name = typeof(RegisterContext).Assembly.FullName;
        services.AddDbContext<RegisterContext>(options => options.UseOracle(configuration.GetConnectionString("SqlConnectionString"), builder => builder.MigrationsAssembly(name)));
        return services;
    }
}