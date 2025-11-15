using Domain.Entities;
using Repository.Persistency.Generic;
using Microsoft.EntityFrameworkCore;
using Repository.Abastractions;
using System.Linq.Expressions;
using Domain.Core.ValueObject;
using Infrastructure.DatabaseContexts;

namespace Repository.Persistency.Implementations;
public class CategoriaRepositorioImpl : BaseRepository<Categoria>, IRepositorio<Categoria>
{
    public RegisterContext Context { get; }
    public CategoriaRepositorioImpl(RegisterContext context) : base(context)
    {
        Context = context;
    }

    public override Categoria Get(Guid id)
    {
        return Context.Categoria.Include(d => d.TipoCategoria).Include(d => d.Usuario).FirstOrDefault(d => d.Id.Equals(id));
    }

    public override List<Categoria> GetAll()
    {
        return Context.Categoria.Include(d => d.TipoCategoria).Include(d => d.Usuario).ToList();
    }

    public override void Insert(Categoria entity)
    {
        var tipoCategoriaId = entity?.TipoCategoria?.Id;
        entity.TipoCategoria = Context.Set<TipoCategoria>().First(tc => tc.Id.Equals(tipoCategoriaId));
        Context.Categoria.Add(entity);
        Context.SaveChanges();
    }

    public override void Update(Categoria entity)
    {
        var tipoCategoriaId = entity?.TipoCategoria?.Id;
        entity.TipoCategoria = Context.Set<TipoCategoria>().First(tc => tc.Id.Equals(tipoCategoriaId));
        var existingEntity = Context.Categoria.Find(entity.Id) ?? throw new();
        Context?.Entry(existingEntity).CurrentValues.SetValues(entity);
        Context?.SaveChanges();
    }

    public override IEnumerable<Categoria> Find(Expression<Func<Categoria, bool>> expression)
    {
        return Context.Categoria.Include(d => d.TipoCategoria).Include(d => d.Usuario).Where(expression);
    }

}