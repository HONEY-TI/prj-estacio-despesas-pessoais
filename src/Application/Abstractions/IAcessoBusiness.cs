using Application.Dtos.Abstractions;
using Application.Dtos.Core;

namespace Application.Abstractions;

public interface IAcessoBusiness<DtoCa, DtoLogin> where DtoCa : class where DtoLogin : class
{
    Task<AuthenticationDto> ValidateCredentials(DtoLogin loginDto);
    Task<AuthenticationDto> ValidateExternalCredentials(GoogleAuthenticationDto authenticationDto);
    Task<AuthenticationDto> ValidateCredentials(string refreshToken);
    Task Create(DtoCa acessoDto);
    Task ChangePassword(Guid idUsuario, string password);
    Task RecoveryPassword(string email);
    Task RevokeToken(Guid idUsurio);
}