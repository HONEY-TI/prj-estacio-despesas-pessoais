using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Authentication;
using Despesas.Application.Authentication.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Abstractions;
using Despesas.Application.Dtos.Core;
using Despesas.Infrastructure.Email.Abstractions;
using Domain.Entities;
using EasyCryptoSalt;
using Repository.Persistency.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static Domain.Core.ValueObject.PerfilUsuario;

namespace Despesas.Application.Implementations;
public class AcessoBusinessImpl<DtoCa, DtoLogin> : IAcessoBusiness<DtoCa, DtoLogin> where DtoCa : AcessoDto where DtoLogin : LoginDto, new()
{
    private readonly ICrypto _crypto;
    private readonly IMapper _mapper;
    private readonly IAcessoRepositorioImpl _repositorio;
    private readonly IEmailSender _emailSender;
    private readonly ISigningConfigurations _singingConfiguration;
    

    public AcessoBusinessImpl(IMapper mapper, IAcessoRepositorioImpl repositorio, SigningConfigurations singingConfiguration, IEmailSender emailSender, ICrypto crypto)
    {
        _mapper = mapper;
        _repositorio = repositorio;
        _singingConfiguration = singingConfiguration;
        _emailSender = emailSender;
        _crypto = crypto;
    }

    public void Create(DtoCa acessoDto)
    {
        var usuario = _mapper.Map<Usuario>(acessoDto);
        usuario = new Usuario().CreateUsuario(usuario);
        Acesso acesso = acessoDto != null ? _mapper.Map<Acesso>(acessoDto) : new Acesso();
        acesso.CreateAccount(usuario, _crypto.Encrypt(acessoDto.Senha));
        _repositorio.Create(acesso);
    }

    public AuthenticationDto ValidateCredentials(DtoLogin loginDto)
    {
        Acesso baseLogin = _repositorio.Find(c => c.Login.Equals(loginDto.Email)) ?? throw new ArgumentException("Usuário inexistente!");

        if (baseLogin?.Usuario?.StatusUsuario == StatusUsuario.Inativo)
            return AuthenticationException("Usuário Inativo!");

        if (!_crypto.Verify(loginDto?.Senha ?? "", baseLogin?.Senha ?? ""))
            return AuthenticationException("Senha inválida!");

        bool credentialsValid = baseLogin is not null && loginDto?.Email == baseLogin.Login;
        if (credentialsValid && baseLogin is not null)
        {
            baseLogin.RefreshToken = _singingConfiguration.GenerateRefreshToken();
            baseLogin.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_singingConfiguration.TokenConfiguration.DaysToExpiry);
            _repositorio.RefreshTokenInfo(baseLogin);
            return AuthenticationSuccess(baseLogin);
        }
        return AuthenticationException("Usuário Inválido!");
    }

    public AuthenticationDto ValidateExternalCredentials(GoogleAuthenticationDto authenticationDto)
    {
        Acesso? baseLogin = _repositorio.Find(c =>
            (c.ExternalProvider == authenticationDto.ExternalProvider && c.ExternalId == authenticationDto.ExternalId) && c.Login.Equals(authenticationDto.Email)
        );

        if (baseLogin == null && !string.IsNullOrEmpty(authenticationDto.ExternalProvider))
        {
            var dtoCa = this._mapper.Map<DtoCa>(authenticationDto);
            dtoCa.Senha = Guid.NewGuid().ToString("N");
            Create(dtoCa);

            baseLogin = _repositorio.Find(c => c.Login.Equals(dtoCa.Email));
        }

        bool credentialsValid = baseLogin is not null && authenticationDto.Email == baseLogin.Login;
        if (credentialsValid && baseLogin is not null)
        {
            baseLogin.RefreshToken = _singingConfiguration.GenerateRefreshToken();
            baseLogin.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_singingConfiguration.TokenConfiguration.DaysToExpiry);
            _repositorio.RefreshTokenInfo(baseLogin);
            return AuthenticationSuccess(baseLogin);
        }
        return AuthenticationException("Usuário Inválido!");
    }

    public AuthenticationDto ValidateCredentials(string refreshToken)
    {
        var baseLogin = _repositorio.FindByRefreshToken(refreshToken);
        var credentialsValid =
            baseLogin is not null
            && baseLogin.RefreshTokenExpiry >= DateTime.UtcNow
            && refreshToken.Equals(baseLogin.RefreshToken)
            && _singingConfiguration.ValidateRefreshToken(refreshToken);

        if (credentialsValid && baseLogin is not null)
            return AuthenticationSuccess(baseLogin);
        else if (baseLogin is not null)
            this.RevokeToken(baseLogin.Id);

        return AuthenticationException("Refresh Token Inválido!");
    }

    public void RevokeToken(Guid idUsuario)
    {
        _repositorio.RevokeRefreshToken(idUsuario);
    }

    public void RecoveryPassword(string email)
    {
        string? newPassword = null;
        Acesso? acesso = null;

        try
        {
            CheckIfUserIsTeste(_repositorio.Find(accout => accout.Login.Equals(email)).Id);
            IsValidEmail(email);
            newPassword = Guid.NewGuid().ToString().Substring(0, 8);
            _repositorio.RecoveryPassword(email, newPassword);
            acesso = _repositorio.Find(c => c.Login.Equals(email));
        }
        catch
        {
            _emailSender.SendEmailPassword(acesso?.Usuario, newPassword);
            throw new ArgumentException("Erro ao enviar email de recuperação de senha!");
        }
    }

    public void ChangePassword(Guid idUsuario, string password)
    {
        CheckIfUserIsTeste(idUsuario);
        _repositorio.ChangePassword(idUsuario, _crypto.Encrypt(password));
    }

    private AuthenticationDto AuthenticationException(string message)
    {
        throw new ArgumentException(message);
    }

    private AuthenticationDto AuthenticationSuccess(Acesso acesso)
    {
        ClaimsIdentity identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim("role",  ((Perfil)acesso.Usuario.PerfilUsuario.Id).ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, acesso.UsuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.AuthTime, acesso.RefreshTokenExpiry == null ? DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() : acesso.RefreshTokenExpiry.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        });

        DateTime createDate = DateTime.Now;
        DateTime expirationDate = createDate + TimeSpan.FromSeconds(_singingConfiguration.TokenConfiguration.Seconds);
        string token = _singingConfiguration.CreateAccessToken(identity);

        return new AuthenticationDto
        {
            Authenticated = true,
            Created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
            Expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
            AccessToken = token,
            RefreshToken = acesso.RefreshToken
        };
    }

    private static void IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser em branco ou nulo!");

        if (email.Length > 256)
            throw new ArgumentException("Email inválido!");

        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        Regex regex = new Regex(pattern);

        if (!regex.IsMatch(email))
            throw new ArgumentException("Email inválido!");
    }

    private void CheckIfUserIsTeste(Guid userIdentity)
    {
        var isUserTeste = _repositorio.Find(accout => accout.Login.Contains("teste@teste.com") && accout.UsuarioId == userIdentity);
        if (isUserTeste is not null)
            throw new ArgumentException("A senha deste usuário não pode ser atualizada!");
    }
}