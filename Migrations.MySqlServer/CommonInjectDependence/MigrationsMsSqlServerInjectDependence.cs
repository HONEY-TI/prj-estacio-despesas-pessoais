using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;

namespace Migrations.MySqlServer.CommonInjectDependence;
public static class MigrationsMsSqlServerInjectDependence
{
    public static IServiceCollection ConfigureMySqlServerMigrationsContext(this IServiceCollection services, IConfiguration configuration)
    {
        var name = typeof(RegisterContext).Assembly.FullName;
        services.AddDbContext<RegisterContext>(options => options.UseMySQL(configuration.GetConnectionString("SqlConnectionString"), builder => builder.MigrationsAssembly(name)));
        return services;
    }
}