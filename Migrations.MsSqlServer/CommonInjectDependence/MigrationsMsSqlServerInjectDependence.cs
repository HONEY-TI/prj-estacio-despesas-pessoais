using Despesas.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Migrations.MsSqlServer.CommonInjectDependence;

public static class MigrationsMsSqlServerInjectDependence
{
    public static IServiceCollection ConfigureMsSqlServerMigrationsContext(this IServiceCollection services, IConfiguration configuration)
    {
        var name = typeof(RegisterContext).Assembly.FullName;
        services.AddDbContext<RegisterContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnectionString"), builder => builder.MigrationsAssembly(name)));
        return services;
    }
}