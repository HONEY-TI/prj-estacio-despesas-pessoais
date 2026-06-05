using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.DatabaseMaintenance;

public class SqlServerDatabaseMaintenance : IDatabaseMaintenance
{
    public void Backup()
    {
        // expdp ...
    }

    public void Restore(string backupFile)
    {
        // impdp ...
    }
}
