using Domain.Core.ValueObject;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Persistency.Abstractions;
using System.Linq.Expressions;

namespace Repository.Persistency.Implementations;
public class AcessoRepositorioImpl : IAcessoRepositorioImpl
{
    public RegisterContext Context { get; }
    public AcessoRepositorioImpl(RegisterContext context)
    {
        Context = context;
    }

    public void Create(Acesso acesso)
    {
        var existingEntity = Context.Set<Acesso>().SingleOrDefault(c => c.Login.Equals(acesso.Login));
        if (existingEntity != null) throw new ArgumentException("Usuário já cadastrado!");

        try
        {
            acesso.Usuario.PerfilUsuario = Context.Set<PerfilUsuario>().First(perfil => perfil.Id.Equals(acesso.Usuario.PerfilUsuario.Id));
            acesso.Usuario.Categorias.ToList().ForEach(c => c.TipoCategoria = Context.Set<TipoCategoria>().First(tc => tc.Id.Equals(c.TipoCategoria.Id)));
            Context.Add(acesso);
            Context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("AcessoRepositorioImpl_Create_Exception", ex);
        }

    }

    public bool RecoveryPassword(string email, string newPassword)
    {
        try
        {
            var entity = Context.Set<Acesso>().First(c => c.Login.Equals(email));
            var acesso = entity as Acesso;
            acesso.Senha = newPassword;
            Context.Acesso.Entry(entity).CurrentValues.SetValues(acesso);
            Context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ChangePassword(Guid idUsuario, string password)
    {
        var usuario = Context.Set<Usuario>().SingleOrDefault(prop => prop.Id.Equals(idUsuario));
        if (usuario is null) return false;

        try
        {
            var acesso = Context.Set<Acesso>().First(c => c.Login.Equals(usuario.Email));
            acesso.Senha = password;
            Context.Acesso.Update(acesso);
            Context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("ChangePassword_Erro", ex);
        }
    }

    public void RevokeRefreshToken(Guid idUsuario)
    {
        var acesso = Context.Acesso.SingleOrDefault(prop => prop.Id.Equals(idUsuario));
        if (acesso is null) throw new ArgumentException("Token inexistente!");
        acesso.RefreshToken = String.Empty;
        acesso.RefreshTokenExpiry = null;
        Context.SaveChanges();
    }

    public Acesso FindByRefreshToken(string refreshToken)
    {
        return Context.Acesso
            .Include(x => x.Usuario)
            .ThenInclude(u => u.PerfilUsuario)
            .First(prop => prop.RefreshToken.Equals(refreshToken));
    }

    public void RefreshTokenInfo(Acesso acesso)
    {
        Context.Acesso.Update(acesso);
        Context.SaveChanges();
    }

    public Acesso? Find(Expression<Func<Acesso, bool>> expression)
    {
        return Context.Acesso.Include(c => c.Usuario).Include(c => c.Usuario.PerfilUsuario).SingleOrDefault(expression);
    }
}