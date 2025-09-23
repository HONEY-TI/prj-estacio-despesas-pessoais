using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.Implementations;

public class DataSeeder : IDataSeeder
{
    private readonly IEnumerable<ISeeder> _seeders;
    private readonly IEnumerable<IUpdater> _updaters;
    private readonly IDatabaseMaintenance _dbMaintenance;

    public DataSeeder(
        IEnumerable<ISeeder> seeders,
        IEnumerable<IUpdater> updaters,
        IDatabaseMaintenance dbMaintenance)
    {
        _seeders = seeders;
        _updaters = updaters;
        _dbMaintenance = dbMaintenance;
    }

    public void Insert()
    {
        foreach (var seeder in _seeders)
            seeder.Seed();
    }

    public void Update()
    {
        foreach (var updater in _updaters)
            updater.Update();
    }

    public void BackupDatabase()
    {
        _dbMaintenance.Backup();
    }

    public void RestoreDatabase(string file)
    {
        _dbMaintenance.Restore(file);
    }
}
