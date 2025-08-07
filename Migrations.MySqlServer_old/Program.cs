using Business.CommonDependenceInject;
using Despesas.DataSeeders.CommonDependenceInject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.CommonDependenceInject;
using System.Reflection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        var environment = context.HostingEnvironment;
        services.AddDbContext<RegisterContext>(options =>
            options.UseMySQL(
                configuration.GetConnectionString("MySqlConnectionString"),
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)
            )
        ); services.AddServicesCryptography(configuration);
        services.AddDataSeeders();
        services.AddRepositories();
        services.AddLogging(config => config.AddConsole());
    })
    .Build();

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
