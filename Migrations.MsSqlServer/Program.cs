using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository.CommonDependenceInject;
using Microsoft.EntityFrameworkCore;
using Migrations.DataSeeders.CommonDependenceInject;
using Despesas.Application.CommonDependenceInject;
using Despesas.Repository.Mapping.Abstractions;
using Despesas.Infrastructure.DatabaseContexts;
using System.Reflection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var provider = DatabaseProvider.SqlServer;
        services.AddSingleton(typeof(DatabaseProvider), provider);

        string connectionString = context.Configuration.GetConnectionString("SqlConnectionString")
              ?? throw new Exception("Connection string 'SqlConnectionString' não encontrada no appsettings.json.");

        string environment = context.Configuration["Environment"] ?? "Production";
        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine($"Connection String: {connectionString}");

        services.AddDbContext<RegisterContext>((sp, options) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            options.UseLoggerFactory(loggerFactory);
            options.UseLazyLoadingProxies();
        });

        services.AddScoped<RegisterContext>(sp =>
        {
            var options = sp.GetRequiredService<DbContextOptions<RegisterContext>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new RegisterContext(options, provider, loggerFactory);
        });

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
