namespace Despesas.Backend.CommonDependenceInject;

public static class HealthCheckDependencyInject
{
    public static void AddHealthCheckConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddUrlGroup(new Uri("https://alexfariakof.com"), name: "Balanço Positivo")
            .AddUrlGroup(new Uri("https://alexfariakof.com/coveragereport"), name: "Relátorio de cobertura de códgio")
            .AddUrlGroup(new Uri("https://alexfariakof.com:42535"), name: "Balanço Positivo Development")
            .AddUrlGroup(new Uri("https://alexfariakof.com:42535"), name: "Doumentação API");

        // -------------------- Health Checks UI --------------------
        builder.Services.AddHealthChecksUI(setup =>
        {
            setup.AddHealthCheckEndpoint("Balanço Positivo Health", "/health");
        })
        .AddInMemoryStorage(); // Armazena resultados em memória

    }
}

