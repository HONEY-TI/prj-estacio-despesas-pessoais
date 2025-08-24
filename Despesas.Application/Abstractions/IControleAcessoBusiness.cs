using Despesas.Application.Dtos.Abstractions;
using Despesas.Application.Dtos.Core;

namespace Despesas.Application.Abstractions;

public interface IAcessoBusiness<DtoCa, DtoLogin> where DtoCa : class where DtoLogin : class
{
    AuthenticationDto ValidateCredentials(DtoLogin loginDto);

    AuthenticationDto ValidateExternalCredentials(GoogleAuthenticationDto authenticationDto);
    
    AuthenticationDto ValidateCredentials(string refreshToken);
    void Create(DtoCa acessoDto);
    void ChangePassword(Guid idUsuario, string password);
    void RecoveryPassword(string email);
    void RevokeToken(Guid idUsurio);
}