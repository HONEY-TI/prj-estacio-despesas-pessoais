using Migrations.DataSeeders.Abstractions;
using Microsoft.Extensions.Logging;

namespace Migrations.DataSeeders.Implementations;

public interface ISeederExceptionHandler
{
    void Handle(string component, Exception exception);
}

public class DataSeeder : IDataSeeder
{
    private readonly IEnumerable<ISeeder> _seeders;
    private readonly IEnumerable<IUpdater> _updaters;
    private readonly IDatabaseMaintenance _dbMaintenance;
    private readonly ISeederExceptionHandler _exceptionHandler;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        IEnumerable<ISeeder> seeders,
        IEnumerable<IUpdater> updaters,
        IDatabaseMaintenance dbMaintenance,
        ISeederExceptionHandler exceptionHandler,
        ILogger<DataSeeder> logger)
    {
        _seeders = seeders;
        _updaters = updaters;
        _dbMaintenance = dbMaintenance;
        _exceptionHandler = exceptionHandler;
        _logger = logger;
    }

    public void Insert()
    {
        foreach (var seeder in _seeders)
        {
            ExecuteSeeder(seeder);
        }
    }

    public void Update()
    {
        foreach (var updater in _updaters)
        {
            ExecuteUpdater(updater);
        }
    }

    public void BackupDatabase()
    {
        _dbMaintenance.Backup();
    }

    public void RestoreDatabase(string file)
    {
        _dbMaintenance.Restore(file);
    }

    private void ExecuteSeeder(ISeeder seeder)
    {
        try
        {
            seeder.Seed();

            _logger.LogInformation(
                "Seeder {Seeder} executed successfully.",
                seeder.GetType().Name);
        }
        catch (Exception exception)
        {
            _exceptionHandler.Handle(
                seeder.GetType().Name,
                exception);
        }
    }

    private void ExecuteUpdater(IUpdater updater)
    {
        try
        {
            updater.Update();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error executing updater {Updater}.",
                updater.GetType().Name);
        }
    }
}