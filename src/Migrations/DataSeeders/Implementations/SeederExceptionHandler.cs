using Microsoft.Extensions.Logging;
using MySqlConnector;
using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Implementations;

public class SeederExceptionHandler : ISeederExceptionHandler
{
    private readonly ILogger<SeederExceptionHandler> _logger;

    public SeederExceptionHandler(
        ILogger<SeederExceptionHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(
        string component,
        Exception exception)
    {
        if (IsDuplicate(exception))
        {
            _logger.LogWarning(
                "Seeder {Component} skipped. Data already exists.",
                component);

            return;
        }

        _logger.LogError(
            exception,
            "Error executing seeder {Component}.",
            component);
    }

    private static bool IsDuplicate(Exception exception)
    {
        return exception.InnerException switch
        {
            MySqlException mysqlException 
                when mysqlException.Number == 1062 => true,

            _ => false
        };
    }
}