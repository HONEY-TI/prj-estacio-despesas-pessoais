using static WebApi.Options.HealthCheckOptions;

namespace WebApi.CommonDependenceInject;

public static class HealthCheckDependencyInject
{
    public static void AddHealthCheckConfigurations(this WebApplicationBuilder builder)
    {

        // Carregar configurações do appsettings
        var healthCheckConfig = builder.Configuration
                        .GetSection("HealthCheckOptions")
                        .Get<HealthCheckConfig>();


        var healthChecksBuilder = builder.Services.AddHealthChecks();
        
        // HealthChecks de URLs externas
        healthCheckConfig?.Endpoints?
            .Where(e => !string.IsNullOrWhiteSpace(e.EndPoint))
            .Where(e => e.Port == null)
            .ToList()
            .ForEach(e =>
            {
                if (Uri.TryCreate(e.EndPoint, UriKind.Absolute, out var uri))
                    healthChecksBuilder.AddUrlGroup(uri, name: e.Name ?? e.EndPoint);
            });

        // HealthChecks de bancos de dados
        healthCheckConfig?.Endpoints?
            .Where(e => e.Port == 3306)
            .ToList()
            .ForEach(e =>
            {
                var connectionString = $"Server={e.EndPoint};Port={e.Port};Database={e.Database};Uid={e.User};Pwd={e.Password};";
                healthChecksBuilder.AddMySql(connectionString, name: e.Name ?? e.EndPoint);
            });

        // -------------------- Health Checks UI --------------------
        builder.Services.AddHealthChecksUI().AddInMemoryStorage();

    }
}

