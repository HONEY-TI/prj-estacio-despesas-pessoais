using Business.CommonDependenceInject;
using Despesas.DataSeeders.CommonDependenceInject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.CommonDependenceInject;
using System;
using System.Reflection;
using ConfigurationManager = System.Configuration.ConfigurationManager;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"]?.ConnectionString
            ?? throw new Exception("Connection string 'MySqlConnectionString' não encontrada no app.config.");

        string environment = ConfigurationManager.AppSettings["Environment"] ?? "Production";

        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine($"Connection String: {connectionString}");

        services.AddDbContext<RegisterContext>(options =>
            options.UseMySQL(
                connectionString,
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)
            )
        );

        var cryptoKey = ConfigurationManager.AppSettings["CryptoConfigurations:Key"];
        var cryptoAuthSalt = ConfigurationManager.AppSettings["CryptoConfigurations:AuthSalt"];

        if (string.IsNullOrEmpty(cryptoKey) || string.IsNullOrEmpty(cryptoAuthSalt))
            throw new Exception("CryptoConfigurations não encontradas no App.config.");

        var inMemoryConfig = new Dictionary<string, string>
        {
            { "CryptoConfigurations:Key", cryptoKey },
            { "CryptoConfigurations:AuthSalt", cryptoAuthSalt }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();

        services.AddServicesCryptography(configuration);
        services.AddDataSeeders();
        services.AddRepositories();

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
