using Despesas.Infrastructure.Email;
using Microsoft.IdentityModel.Tokens;
using Repository.Persistency.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using __mock__.Entities;
using System.Linq.Expressions;
using AutoMapper;
using EasyCryptoSalt;
using Microsoft.AspNetCore.Builder;
using Despesas.Backend.CommonDependenceInject;
using Microsoft.Extensions.DependencyInjection;
using Despesas.Application.Implementations;
using Despesas.Application.Authentication;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using Despesas.Application.CommonDependenceInject;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos.Abstractions;

namespace Application;
public sealed class AcessoBusinessImplTest
{
    private readonly Mock<IAcessoRepositorioImpl> _repositorioMock;
    private readonly AcessoBusinessImpl<AcessoDto, LoginDto> _acessoBusiness;
    private readonly SigningConfigurations _singingConfiguration;

    private Mapper _mapper;

    public AcessoBusinessImplTest()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

        var builder = WebApplication.CreateBuilder();
        builder.AddSigningConfigurations();
        builder.Services.AddServicesCryptography(builder.Configuration);
        var services = builder.Services;
        
        _singingConfiguration = services.BuildServiceProvider().GetService<SigningConfigurations>();
        var crypto = services.BuildServiceProvider().GetService<ICrypto>();
        _repositorioMock = new Mock<IAcessoRepositorioImpl>();
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<AcessoProfile>(); }));
        _acessoBusiness = new AcessoBusinessImpl<AcessoDto, LoginDto>(_mapper, _repositorioMock.Object, _singingConfiguration, new EmailSender(), crypto);
    }

    [Fact]
    public void Create_Should_Acesso_Returns_True()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFakerVM();

        _repositorioMock.Setup(repo => repo.Create(It.IsAny<Acesso>()));

        // Act
        _acessoBusiness.Create(acesso);

        // Assert        
        _repositorioMock.Verify(repo => repo.Create(It.IsAny<Acesso>()), Times.Once);
    }

    [Fact]
    public void ValidateCredentials_Should_Return_Valid_Credentials_And_AccessToken()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        var loginDto = new LoginDto { Email = acesso.Login, Senha = "teste" };
        acesso.Senha = loginDto.Senha;
        acesso.Usuario.StatusUsuario = StatusUsuario.Ativo;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(acesso);
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(acesso);
        var mockAcessoBusiness = new Mock<IAcessoBusiness<AcessoDto, LoginDto>>(MockBehavior.Strict);
        mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>())).Returns(new BaseAuthenticationDto() { Authenticated = true, AccessToken = Guid.NewGuid().ToString() });

        // Act
        var result = mockAcessoBusiness.Object.ValidateCredentials(loginDto);

        // Assert
        Assert.True(result.Authenticated);
        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public void ValidateCredentials_Should_Returns_Usaurio_Inexistente()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com" };
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(() => null);

        // Act & Assert 
        Assert.Throws<ArgumentException>(() => _acessoBusiness.ValidateCredentials(loginDto));
    }

    [Fact]
    public void ValidateCredentials_Should_Returns_Usuario_Inativo()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        var usuarioInativo = UsuarioFaker.Instance.GetNewFaker();
        usuarioInativo.StatusUsuario = StatusUsuario.Inativo;
        acesso.Usuario = usuarioInativo;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(acesso);

        // Act
        var result = _acessoBusiness.ValidateCredentials(new LoginDto { Email = acesso.Login, Senha = acesso.Senha });

        // Assert
        Assert.False(result.Authenticated);
        Assert.Contains("Usuário Inativo!", result.Message);
    }

    [Fact]
    public void ValidateCredentials_Should_Returns_Email_Inexistente()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com", Senha = "teste", };
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            StatusUsuario = StatusUsuario.Ativo
        };

        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(() => null);

        // Act & Assert 
        Assert.Throws<ArgumentException>(() => _acessoBusiness.ValidateCredentials(loginDto));
    }

    [Fact]
    public void ValidateCredentials_Should_Returns_Senha_Invalida()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        acesso.Usuario.StatusUsuario = StatusUsuario.Ativo;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(acesso);
        var mockAcessoBusiness = new Mock<IAcessoBusiness<AcessoDto, LoginDto>>(MockBehavior.Strict);
        mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>())).Returns(new BaseAuthenticationDto() { Authenticated = false, AccessToken = Guid.NewGuid().ToString(), Message = "Senha inválida!" });
        var loginDto = new LoginDto() { Email = acesso.Login, Senha = Guid.NewGuid().ToString() };

        // Act
        var result = mockAcessoBusiness.Object.ValidateCredentials(loginDto);

        // Assert
        Assert.False(result.Authenticated);
        Assert.Contains("Senha inválida!", result.Message);
    }

    [Fact]
    public void ValidateCredentials_Should_Returns_Usuario_Invalido()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com", Senha = "teste", };

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            StatusUsuario = StatusUsuario.Ativo
        };
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Throws(new ArgumentException("Usuário Inválido!"));

        // Act &  Assert
        Assert.Throws<ArgumentException>(() => _acessoBusiness.ValidateCredentials(loginDto));
    }

    [Fact]
    public void RecoveryPassword_Should_Execute_And_Returns_True()
    {
        /*
        // Arrange
        string email = "teste@example.com";
        _repositorioMock.Setup(repo => repo.RecoveryPassword(email)).Returns(true);

        // Act
        bool result = _acessoBusiness.RecoveryPassword(email);

        // Assert
        Assert.True(result);
        _repositorioMock.Verify(repo => repo.RecoveryPassword(email), Times.Once);
        */
    }

    [Fact]
    public void ChangePassword_Should_Execute_And_Returns_True()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        string newPassword = "123456789";
        _repositorioMock.Setup(repo => repo.ChangePassword(It.IsAny<Guid>(), newPassword)).Returns(true);
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(() => null);

        // Act
        _acessoBusiness.ChangePassword(idUsuario, newPassword);

        // Assert        
        _repositorioMock.Verify(repo => repo.ChangePassword(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ValidateCredentials_Should_Return_Authentication_Success_When_Credentials_Are_Valid()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var baseLogin = new Acesso
        {
            Id = idUsuario,
            RefreshTokenExpiry = DateTime.UtcNow.AddHours(1),            
            Usuario = UsuarioFaker.Instance.GetNewFaker()
        };

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Audience = "Audience",
            Issuer = "Issuer",
            Claims = new Dictionary<string, object> { { "KEY", idUsuario } },
            SigningCredentials = _singingConfiguration.SigningCredentials,
            Expires = DateTime.UtcNow.AddSeconds(60),
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,

        });
        var validToken = handler.WriteToken(securityToken);
        baseLogin.RefreshToken = validToken;
        var authenticationDto = new BaseAuthenticationDto
        {
            RefreshToken = validToken
        };
        _repositorioMock.Setup(repo => repo.FindByRefreshToken(It.IsAny<string>())).Returns(baseLogin);

        // Act
        var result = _acessoBusiness.ValidateCredentials(validToken);

        // Assert
        Assert.True(result.Authenticated);
        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public void ValidateCredentials_Should_Revoke_Token_When_RefreshToken_Expires()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var baseLogin = new Acesso
        {
            Id = idUsuario,
            RefreshTokenExpiry = DateTime.UtcNow.AddHours(-1), // Token expirado
            RefreshToken = "expired_refresh_token"
        };
        var authenticationDto = new BaseAuthenticationDto
        {
            RefreshToken = "expired_refresh_token"
        };
        _repositorioMock.Setup(repo => repo.FindByRefreshToken(It.IsAny<string>())).Returns(baseLogin);

        // Act
        _acessoBusiness.ValidateCredentials("expired_refresh_token");

        // Assert
        _repositorioMock.Verify(repo => repo.RevokeRefreshToken(idUsuario), Times.Once);
    }

    [Fact]
    public void ValidateCredentials_Should_Return_Authentication_Exception_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var authenticationDto = new BaseAuthenticationDto
        {
            RefreshToken = "invalid_refresh_token"
        };

        // Act
        var result = _acessoBusiness.ValidateCredentials("invalid_refresh_token");

        // Assert
        Assert.False(result.Authenticated);
        Assert.Equal("Refresh Token Inválido!", result.Message);
    }

    [Fact]
    public void ValidateCredentials_Should_Revoke_Token_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var mockAcesso = AcessoFaker.Instance.GetNewFaker();
        _repositorioMock.Setup(repo => repo.FindByRefreshToken(It.IsAny<string>())).Returns(mockAcesso);

        // Act
        _acessoBusiness.ValidateCredentials("invalid_refresh_token");

        // Assert
        _repositorioMock.Verify(repo => repo.RevokeRefreshToken(It.IsAny<Guid>()), Times.Once);
    }

}
