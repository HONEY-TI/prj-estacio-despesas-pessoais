using Infrastructure.DatabaseContexts;
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

    public async Task Create(Acesso acesso)
    {
        acesso.Usuario.PerfilUsuario = Context.PerfilUsuario.First(p => p.Id == acesso.Usuario.PerfilUsuario.Id);
        acesso.Usuario.Categorias.ToList().ForEach(c =>
        {
            var tipoCategoria = Context.TipoCategoria.FirstOrDefault(tc => tc.Id == c.TipoCategoria.Id);
            if (tipoCategoria != null)
                c.TipoCategoria = tipoCategoria;
        }); 
        Context.Acesso.Add(acesso);
        await Context.SaveChangesAsync();
    }

    public async Task RecoveryPassword(string email, string newPassword)
    {
        var entity = Context.Acesso.First(c => c.Login.Equals(email));
        entity.Senha = newPassword;
        Context.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task ChangePassword(Guid idUsuario, string password)
    {
        var usuario = Context.Acesso.Include(u => u.Usuario).SingleOrDefault(prop => prop.UsuarioId.Equals(idUsuario))
            ?? throw new();

        var acesso = Context.Acesso.First(c => c.Login.Equals(usuario.Login));
        acesso.Senha = password;
        Context.Acesso.Update(acesso);
        await Context.SaveChangesAsync();
    }

    public async Task RevokeRefreshToken(Guid idUsuario)
    {
        var acesso = Context.Acesso.FirstOrDefault(prop => prop.Id.Equals(idUsuario));
        if (acesso is null) throw new ArgumentException("Token inexistente!");
        acesso.RefreshToken = String.Empty;
        acesso.RefreshTokenExpiry = null;
        Context.Acesso.Update(acesso);
        await Context.SaveChangesAsync();
    }

    public async Task<Acesso> FindByRefreshToken(string refreshToken)
    {
        return await Context.Acesso
            .Include(x => x.Usuario)
            .ThenInclude(u => u.PerfilUsuario)
            .FirstAsync(prop => prop.RefreshToken.Equals(refreshToken));
    }

    public async Task RefreshTokenInfo(Acesso acesso)
    {
        Context.Acesso.Update(acesso);
        await Context.SaveChangesAsync();
    }

    public async Task<Acesso?> Find(Expression<Func<Acesso, bool>> expression)
    {
        return await Context.Acesso.Include(c => c.Usuario).Include(c => c.Usuario.PerfilUsuario).SingleOrDefaultAsync(expression);
    }
}