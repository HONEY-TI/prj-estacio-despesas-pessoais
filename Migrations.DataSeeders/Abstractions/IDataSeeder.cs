namespace Migrations.DataSeeders;

public interface IDataSeeder
{
    void Insert();
    void Update();
    void BackupDatabase();
    void RestoreDatabase(string file);
}