using Domain.Entities;
using System.Linq.Expressions;

namespace Repository.Persistency.Abstractions;
public interface IAcessoRepositorioImpl
{
    Task Create(Acesso acesso);
    Task<Acesso?> Find(Expression<Func<Acesso, bool>> expression);
    Task ChangePassword(Guid idUsuario, string password);
    Task RecoveryPassword(string email, string newPassword);
    Task RevokeRefreshToken(Guid idUsuario);
    Task<Acesso> FindByRefreshToken(string refreshToken);
    Task RefreshTokenInfo(Acesso acesso);    
}
