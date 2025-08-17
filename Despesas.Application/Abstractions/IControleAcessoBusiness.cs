using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Abstractions;

namespace Despesas.Application.Abstractions;

public interface IAcessoBusiness<DtoCa, DtoLogin> where DtoCa : class where DtoLogin : class
{
    BaseAuthenticationDto ValidateCredentials(DtoLogin loginDto);

    BaseAuthenticationDto ValidateExternalCredentials(GoogleAuthenticationDto authenticationDto);
    
    BaseAuthenticationDto ValidateCredentials(string refreshToken);
    void Create(DtoCa acessoDto);
    void ChangePassword(Guid idUsuario, string password);
    void RecoveryPassword(string email);
    void RevokeToken(Guid idUsurio);
}