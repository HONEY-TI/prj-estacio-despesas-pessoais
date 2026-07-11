namespace Migrations.DataSeeders.Abstractions;

public interface ISeederExceptionHandler
{
    void Handle(string component, Exception exception);
}
