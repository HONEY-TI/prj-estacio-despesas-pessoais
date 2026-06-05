namespace Repository.UnitOfWork.Abstractions;
public interface IUnitOfWork<T> where T : class
{
    IUnitOfWorkRepository<T> Repository { get; }
    Task CommitAsync();
}
