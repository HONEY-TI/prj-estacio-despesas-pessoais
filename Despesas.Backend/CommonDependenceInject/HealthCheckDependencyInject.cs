using static Despesas.Backend.Options.HealthCheckOptions;

namespace Despesas.Backend.CommonDependenceInject;

public static class HealthCheckDependencyInject
{
    public static void AddHealthCheckConfigurations(this WebApplicationBuilder builder)
    {

        // Carregar configurações do appsettings
        var healthCheckConfig = builder.Configuration
                        .GetSection("HealthCheckOptions")
                        .Get<HealthCheckConfig>();


        var healthChecksBuilder = builder.Services.AddHealthChecks();

        healthCheckConfig?.Endpoints?
            .Where(e => !string.IsNullOrWhiteSpace(e.EndPoint))
            .ToList()
            .ForEach(e =>
            {
                if (e.Port is null)
                {
                    if (Uri.TryCreate(e.EndPoint, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    {
                        healthChecksBuilder.AddUrlGroup(uri, name: e.Name ?? e.EndPoint);
                    }
                    else
                    {
                        Console.WriteLine($"Ignorando endpoint inválido: {e.EndPoint}");
                    }
                }
                else if (e.Port == 3306)
                {
                    if (string.IsNullOrWhiteSpace(e.User) || string.IsNullOrWhiteSpace(e.Password))
                    {
                        Console.WriteLine($"Ignorando banco sem usuário/senha: {e.EndPoint}");
                        return;
                    }
                    var connectionString = $"Server={e.EndPoint};Port={e.Port};Database={e.Database};Uid={e.User};Pwd={e.Password};SslMode=none;";
                    healthChecksBuilder.AddMySql(connectionString, name: e.Name ?? e.EndPoint);
                }
            });


        // -------------------- Health Checks UI --------------------
        builder.Services.AddHealthChecksUI(setup =>
        {
            // Executa os health checks a cada 30 minutos (1800 segundos)
            setup.SetEvaluationTimeInSeconds(1800);

            // Mantém no máximo 20 registros por endpoint para evitar excesso de memória
            setup.MaximumHistoryEntriesPerEndpoint(20);

        })
        .AddInMemoryStorage();

    }
}

