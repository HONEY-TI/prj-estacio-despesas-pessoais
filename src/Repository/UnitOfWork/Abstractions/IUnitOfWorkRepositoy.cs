using System.Linq.Expressions;

namespace Repository.UnitOfWork.Abstractions;

public interface IUnitOfWorkRepository<T> where T : class
{
    Task<T?> Get(Guid entityId);
    Task<IEnumerable<T>> GetAll();
    Task Insert(T entity);
    Task Update(T entity);
    Task Delete(Guid entityId);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression);
}
