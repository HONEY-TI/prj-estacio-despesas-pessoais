using Application.CommonDependenceInject;
using Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Migrations.DataSeeders.CommonDependenceInject;
using Migrations.MySqlServer.CommonInjectDependence;
using Repository.CommonDependenceInject;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.ConfigureMySqlServerMigrationsContext(context.Configuration);

        string environment = context.Configuration["Environment"] ?? "Production";
        Console.WriteLine($"Environment: {environment}");

        var cryptoKey = context.Configuration["CryptoConfigurations:Key"];
        var cryptoAuthSalt = context.Configuration["CryptoConfigurations:AuthSalt"];

        if (string.IsNullOrEmpty(cryptoKey) || string.IsNullOrEmpty(cryptoAuthSalt))
            throw new Exception("CryptoConfigurations não encontradas no App.config.");

        var inMemoryConfig = new Dictionary<string, string>
        {
            { "CryptoConfigurations:Key", cryptoKey },
            { "CryptoConfigurations:AuthSalt", cryptoAuthSalt }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();

        services.AddServicesCryptography(configuration);
        services.AddRepositories();
        services.AddDataSeeders();
        services.AddLogging(logging => logging.AddConsole());
    })
    .Build();

// DI Scope
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<RegisterContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Applying migrations...");
    await context.Database.MigrateAsync();
    logger.LogInformation("Migrations applied successfully.");

    logger.LogInformation("Executing data seeders...");

    services.RunDataSeeders();
    logger.LogInformation("Data seeders executed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    throw;
}

Console.WriteLine("Done.");
