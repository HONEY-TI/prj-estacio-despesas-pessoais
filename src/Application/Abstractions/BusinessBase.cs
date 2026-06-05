using AutoMapper;
using Repository.UnitOfWork.Abstractions;
using Repository.Persistency.Generic;

namespace Application.Abstractions;
public abstract class BusinessBase<Dto, Entity> : IBusinessBase<Dto, Entity> where Dto : class where Entity : class, new()
{
    protected IUnitOfWork<Entity>? UnitOfWork { get; }
    protected IMapper Mapper { get; set; }

    protected IRepositorio<Entity> Repository { get; }

    protected BusinessBase(IMapper mapper, IRepositorio<Entity> repository, IUnitOfWork<Entity> unitOfWork = null)
    {
        Repository = repository;
        Mapper = mapper;
        UnitOfWork = unitOfWork;
    }

    public abstract Task<Dto> Create(Dto dto);

    public virtual Task<Dto> FindById(Guid id)
    {
        throw new NotImplementedException("Este método não foi implementado.");
    }

    public virtual Task<Dto> FindById(Guid id, Guid idUsuario) { return null; }

    public abstract Task<List<Dto>> FindAll(Guid idUsuario);

    public abstract Task<Dto> Update(Dto dto);

    public abstract Task<bool> Delete(Dto dto);
}