using Infrastructure.DatabaseContexts;
using Domain.Core.Aggreggates;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository.UnitOfWork.Abstractions;

public sealed class BaseUnitOfWork<T> : IUnitOfWorkRepository<T> where T : BaseDomain
{
    protected readonly RegisterContext Context;

    public BaseUnitOfWork(RegisterContext context)
    {
        Context = context;
    }

    public async Task<T?> Get(Guid entityId)
    {
        return await Context.Set<T>().FindAsync(entityId);
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public async Task Insert(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
    }

    public async Task Update(T entity)
    {
        var existingEntity = await Context.Set<T>().FirstOrDefaultAsync(c => c.Id.Equals(entity.Id));
        Context?.Entry(existingEntity).CurrentValues.SetValues(entity);
    }

    public async Task Delete(Guid entityId)
    {
        var entity = await Get(entityId);
        if (entity != null)
            Context.Set<T>().Remove(entity);
    }

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
    {
        return await Context.Set<T>().Where(expression).ToListAsync();
    }
}
