namespace Migrations.DataSeeders.Abstractions;
public interface IDatabaseMaintenance
{
    void Backup();
    void Restore(string backupFile);
}