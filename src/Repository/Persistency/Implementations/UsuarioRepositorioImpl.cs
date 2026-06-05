using Infrastructure.DatabaseContexts;
using Domain.Core.ValueObject;
using Domain.Entities;
using EasyCryptoSalt;
using Microsoft.EntityFrameworkCore;
using Repository.Abastractions;
using Repository.Persistency.Generic;

namespace Repository.Persistency.Implementations;
public class UsuarioRepositorioImpl : BaseRepository<Usuario>, IRepositorio<Usuario>
{
    public RegisterContext Context { get; }

    public UsuarioRepositorioImpl(RegisterContext context) : base(context)
    {
        Context = context;
    }

    public override void Insert(Usuario entity)
    {
        var acesso = new Acesso();
        acesso.Usuario = entity;
        acesso.CreateAccount(entity, Crypto.Instance.Encrypt("12345T!"));
        acesso.Usuario.PerfilUsuario = Context.Set<PerfilUsuario>().First(perfil => perfil.Id.Equals(acesso.Usuario.PerfilUsuario.Id));
        acesso.Usuario.Categorias.ToList().ForEach(c => c.TipoCategoria = Context.Set<TipoCategoria>().First(tc => tc.Id.Equals(c.TipoCategoria.Id)));
        Context.Add(acesso);
        Context.SaveChanges();
    }

    public override List<Usuario> GetAll()
    {
        return Context.Usuario.ToList();
    }

    public override Usuario Get(Guid id)
    {

        return Context.Usuario
            .Include(u => u.PerfilUsuario)
            .Single(prop => prop.Id.Equals(id));
    }

    public override void Delete(Usuario obj)
    {
        var result = Context.Usuario.SingleOrDefault(prop => prop.Id.Equals(obj.Id));
        result.StatusUsuario = StatusUsuario.Inativo;
        Context.Update(result);
        Context.SaveChanges();
    }

    public override bool Exists(Guid id)
    {
        return Context.Usuario.Any(prop => prop.Id.Equals(id));
    }
}