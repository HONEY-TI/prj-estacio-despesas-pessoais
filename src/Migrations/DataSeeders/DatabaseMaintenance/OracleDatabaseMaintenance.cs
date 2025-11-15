using Migrations.DataSeeders.Abstractions;

namespace Migrations.DataSeeders.DatabaseMaintenance;

public class OracleDatabaseMaintenance : IDatabaseMaintenance
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
