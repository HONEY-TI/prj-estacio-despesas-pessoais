using AutoMapper;
using Domain.Core.Aggreggates;
using Repository.Persistency.Generic;

namespace Application.Abstractions.Generic;

public class GenericBusiness<Dto, Entity> : IBusiness<Dto, Entity> where Dto : class where Entity : BaseDomain, new()
{
    private readonly IRepositorio<Entity> _repositorio;
    private readonly IMapper _mapper;

    public GenericBusiness(IMapper mapper, IRepositorio<Entity> repositorio)
    {
        _mapper = mapper;
        _repositorio = repositorio;
    }
    public Dto Create(Dto obj)
    {
        var entity = _mapper.Map<Entity>(obj);
        _repositorio.Insert(entity);
        return _mapper.Map<Dto>(entity);
    }

    public List<Dto> FindAll(Guid idUsuario)
    {
        return _mapper.Map<List<Dto>>(_repositorio.GetAll());
    }

    public virtual Dto FindById(Guid id, Guid idUsuario)
    {
        return _mapper.Map<Dto>(_repositorio.Get(id));
    }

    public Dto Update(Dto obj)
    {
        var entity = _mapper.Map<Entity>(obj);
        _repositorio.Update(entity);
        return _mapper.Map<Dto>(obj);
    }

    public bool Delete(Dto obj)
    {
        var entity = _mapper.Map<Entity>(obj);
        _repositorio.Delete(entity);
        return true;
    }
}
