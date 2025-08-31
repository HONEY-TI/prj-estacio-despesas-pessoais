using System.Linq.Expressions;

namespace Repository.Persistency.Generic;
public interface IRepositorio<T> where T : class
{
    public T Get(Guid id);
    public List<T> GetAll();
    public void Insert(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    public bool Exists(Guid id);
    IEnumerable<T>? Find(Expression<Func<T, bool>> expression);
}