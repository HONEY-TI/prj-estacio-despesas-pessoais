using Domain.Entities;
using System.Linq.Expressions;

namespace Repository.Persistency.Abstractions;
public interface IAcessoRepositorioImpl
{
    void Create(Acesso acesso);
    Acesso? Find(Expression<Func<Acesso, bool>> expression);
    bool ChangePassword(Guid idUsuario, string password);
    bool RecoveryPassword(string email, string newPassword);
    void RevokeRefreshToken(Guid idUsuario);
    Acesso FindByRefreshToken(string refreshToken);
    void RefreshTokenInfo(Acesso acesso);    
}
