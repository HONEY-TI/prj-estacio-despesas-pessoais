using Infrastructure.DatabaseContexts;
using Repository.UnitOfWork.Abstractions;
using Domain.Core.Aggreggates;

namespace Repository.UnitOfWork;

public class UnitOfWork<T> : IUnitOfWork<T> where T : BaseDomain, new()
{
    private readonly RegisterContext _context;
    private IUnitOfWorkRepository<T>? _repository;

    public UnitOfWork(RegisterContext context)
    {
        _context = context;
    }

    public IUnitOfWorkRepository<T> Repository
    {
        get
        {
            return _repository ??= new BaseUnitOfWork<T>(_context);
        }
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}
