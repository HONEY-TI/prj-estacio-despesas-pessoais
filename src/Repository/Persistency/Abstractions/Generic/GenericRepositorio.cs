using Infrastructure.DatabaseContexts;
using Domain.Core.Aggreggates;
using System.Linq.Expressions;

namespace Repository.Persistency.Generic;
public class GenericRepositorio<T> : IRepositorio<T> where T : BaseDomain, new()
{
    protected readonly RegisterContext _context;

    public GenericRepositorio(RegisterContext context)
    {
        this._context = context;
    }

    public virtual void Insert(T item)
    {
        this._context.Add(item);
        this._context.SaveChanges();
    }

    public virtual List<T> GetAll()
    {
        return this._context.Set<T>().ToList();
    }

    public virtual T Get(Guid id)
    {
        return this._context.Set<T>().SingleOrDefault(prop => prop.Id.Equals(id));
    }

    public virtual void Update(T entity)
    {
        var existingEntity = _context.Set<T>().Find(entity.Id);
        this._context.Set<T>().Entry(existingEntity).CurrentValues.SetValues(entity);
        this._context.SaveChanges();
    }

    public virtual void Delete(T entity)
    {
        this._context.Set<T>().Remove(entity);
        this._context.SaveChanges();
    }

    public virtual bool Exists(Guid id)
    {
        return this._context.Set<T>().Any(prop => prop.Id.Equals(id));
    }

    public IEnumerable<T>? Find(Expression<Func<T, bool>> expression)
    {
        return this._context.Set<T>().Where(expression);
    }
}