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
        try
        {
            var existingEntity = Context.Acesso
                .Include(a => a.Usuario) 
                .ThenInclude(u => u.PerfilUsuario) 
                .Include(a => a.Usuario.Categorias) 
                .ThenInclude(c => c.TipoCategoria) 
                .SingleOrDefault(c => c.Login.Equals(acesso.Login))
                ?? throw new();

            acesso.Usuario.PerfilUsuario = existingEntity.Usuario.PerfilUsuario;
            acesso.Usuario.Categorias.ToList().ForEach(c => c.TipoCategoria = Context.TipoCategoria.First(tc => tc.Id.Equals(c.TipoCategoria.Id)));
            Context.Add(acesso);
            Context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("AcessoRepositorioImpl_Create_Exception", ex);
        }

    }

    public void RecoveryPassword(string email, string newPassword)
    {
        try
        {
            var entity = Context.Acesso.First(c => c.Login.Equals(email));
            entity.Senha = newPassword;
            Context.Update(entity);
            Context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("RecoveryPassword_Erro", ex);
        }
    }

    public void ChangePassword(Guid idUsuario, string password)
    {
        try
        {
            var usuario = Context.Acesso
                .Include(u => u.Usuario)
                .SingleOrDefault(prop => prop.UsuarioId
                .Equals(idUsuario))
                ?? throw new();

            var acesso = Context.Acesso.First(c => c.Login.Equals(usuario.Login));
            acesso.Senha = password;
            Context.Acesso.Update(acesso);
            Context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("ChangePassword_Erro", ex);
        }
    }

    public void RevokeRefreshToken(Guid idUsuario)
    {
        var acesso = Context.Set<Acesso>().SingleOrDefault(prop => prop.Id.Equals(idUsuario));
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