using Infrastructure.DatabaseContexts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Abastractions;
using Repository.Persistency.Generic;
using System.Linq.Expressions;

namespace Repository.Persistency.Implementations;
public class ReceitaRepositorioImpl : BaseRepository<Receita>, IRepositorio<Receita>
{
    public RegisterContext Context { get; }
    public ReceitaRepositorioImpl(RegisterContext context) : base(context)
    {
        Context = context;
    }

    public override Receita Get(Guid id)
    {
        return Context.Receita.Include(d => d.Categoria).Include(d => d.Usuario).FirstOrDefault(d => d.Id.Equals(id));
    }

    public override List<Receita> GetAll()
    {
        return Context.Receita.Include(d => d.Categoria).Include(d => d.Usuario).ToList();
    }

    public override void Insert(Receita entity)
    {
        var categoriaId = entity.CategoriaId;
        entity.Categoria = Context.Set<Categoria>().First(c => c.Id.Equals(categoriaId));
        Context.Add(entity);
        Context.SaveChanges();
    }

    public override void Update(Receita entity)
    {
        var receitaId = entity.Id;
        var categoriaId = entity.CategoriaId;
        entity.Categoria = Context.Set<Categoria>().First(c => c.Id.Equals(categoriaId));
        var existingEntity = Context.Receita.Single(prop => prop.Id.Equals(receitaId));
        Context?.Entry(existingEntity).CurrentValues.SetValues(entity);
        Context?.SaveChanges();
        entity = existingEntity;
    }

    public override IEnumerable<Receita> Find(Expression<Func<Receita, bool>> expression)
    {
        return Context.Receita
            .Include(d => d.Categoria)
            .ThenInclude(c => c.TipoCategoria)
            .Where(expression);
    }
}