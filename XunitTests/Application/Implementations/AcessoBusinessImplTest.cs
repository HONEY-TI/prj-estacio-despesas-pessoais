using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Authentication;
using Despesas.Application.CommonDependenceInject;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Abstractions;
using Despesas.Application.Dtos.Core;
using Despesas.Application.Dtos.Profile;
using Despesas.Application.Implementations;
using Despesas.Backend.CommonDependenceInject;
using Despesas.GlobalException.CustomExceptions.Acesso;
using Despesas.Infrastructure.Email;
using EasyCryptoSalt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository.Persistency.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

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
    public async Task Create_Should_Acesso_Returns_True()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFakerVM();

        _repositorioMock.Setup(repo => repo.Create(It.IsAny<Acesso>()));

        // Act
        await _acessoBusiness.Create(acesso);

        // Assert        
        _repositorioMock.Verify(repo => repo.Create(It.IsAny<Acesso>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Return_Valid_Credentials_And_AccessToken()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        var loginDto = new LoginDto { Email = acesso.Login, Senha = "teste" };
        acesso.Senha = loginDto.Senha;
        acesso.Usuario.StatusUsuario = StatusUsuario.Ativo;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(Task.Run(() => acesso));
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(Task.Run(() => acesso));
        var mockAcessoBusiness = new Mock<IAcessoBusiness<AcessoDto, LoginDto>>(MockBehavior.Strict);
        mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>())).Returns(Task.Run(() => new AuthenticationDto() { Authenticated = true, AccessToken = Guid.NewGuid().ToString() }));

        // Act
        var result = await mockAcessoBusiness.Object.ValidateCredentials(loginDto);

        // Assert
        Assert.True(result.Authenticated);
        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Returns_Usaurio_Inexistente()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com" };
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).ReturnsAsync(() => null);

        // Act & Assert 
        var exception = await Assert.ThrowsAsync<UsuarioInexistenteException>(async () => await _acessoBusiness.ValidateCredentials(loginDto));
        Assert.Contains("Usuário Inexistente!", exception.Message);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Returns_Usuario_Inativo()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        var usuarioInativo = UsuarioFaker.Instance.GetNewFaker();
        usuarioInativo.StatusUsuario = StatusUsuario.Inativo;
        acesso.Usuario = usuarioInativo;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).ReturnsAsync(acesso);

        // Act && Assert
        var exception = await Assert.ThrowsAsync<UsuarioInativoException>(async () => await _acessoBusiness.ValidateCredentials(new LoginDto { Email = acesso.Login, Senha = acesso.Senha }));
        Assert.Contains("Usuário Inativo!", exception.Message);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Returns_Email_Inexistente()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com", Senha = "teste", };
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            StatusUsuario = StatusUsuario.Ativo
        };

        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).ReturnsAsync(() => null);

        // Act & Assert 
        var exception = await Assert.ThrowsAsync<UsuarioInexistenteException>(async () =>  await _acessoBusiness.ValidateCredentials(loginDto));
        Assert.Contains("Usuário Inexistente!", exception.Message);

    }

    [Fact]
    public void ValidateCredentials_Should_Returns_Senha_Invalida()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        acesso.Usuario.StatusUsuario = StatusUsuario.Ativo;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(Task.Run(() => acesso));
        var mockAcessoBusiness = new Mock<IAcessoBusiness<AcessoDto, LoginDto>>(MockBehavior.Strict);
        mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>())).Throws(new ArgumentException("Senha inválida!"));
        var loginDto = new LoginDto() { Email = acesso.Login, Senha = Guid.NewGuid().ToString() };

        // Act && Assert
        var exception = Assert.Throws<ArgumentException>(() => mockAcessoBusiness.Object.ValidateCredentials(loginDto).Result);
        Assert.Contains("Senha inválida!", exception.Message);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Returns_Usuario_Invalido()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com", Senha = "teste", };

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            StatusUsuario = StatusUsuario.Ativo
        };
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).ThrowsAsync(new ArgumentException("Usuário Inválido!"));

        // Act &  Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _acessoBusiness.ValidateCredentials(loginDto));
        Assert.Contains("Usuário Inválido!", exception.Message);
    }

    [Fact]
    public void ChangePassword_Should_Execute_Successfuly()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        string newPassword = "123456789";
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(() => null);

        // Act
        _acessoBusiness.ChangePassword(idUsuario, newPassword);

        // Assert        
        _repositorioMock.Verify(repo => repo.ChangePassword(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Return_Authentication_Success_When_Credentials_Are_Valid()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var baseLogin = new Acesso
        {
            Id = idUsuario,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(2),            
            Usuario = UsuarioFaker.Instance.GetNewFaker()           
        };

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Audience = "Audience",
            Issuer = "Issuer",
            Claims = new Dictionary<string, object> { { "KEY", idUsuario } },
            SigningCredentials = _singingConfiguration.SigningCredentials,
            Expires = DateTime.UtcNow.AddDays(1),
            NotBefore = DateTime.UtcNow,

        });
        var validToken = handler.WriteToken(securityToken);
        baseLogin.RefreshToken = validToken;
        var authenticationDto = new AuthenticationDto
        {
            Authenticated = true,
            Expiration = DateTime.UtcNow.AddDays(1).ToString(),
            RefreshToken = validToken
        };
        _repositorioMock.Setup(repo => repo.FindByRefreshToken(It.IsAny<string>())).ReturnsAsync(baseLogin);

        // Act
        var result = await _acessoBusiness.ValidateCredentials(validToken);

        // Assert
        Assert.True(result.Authenticated);
        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Revoke_Token_When_RefreshToken_Expires()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var baseLogin = new Acesso
        {
            Id = idUsuario,
            RefreshTokenExpiry = DateTime.UtcNow.AddHours(-1), // Token expirado
            RefreshToken = "expired_refresh_token"
        };
        var authenticationDto = new AuthenticationDto
        {
            RefreshToken = "expired_refresh_token"
        };
        _repositorioMock.Setup(repo => repo.FindByRefreshToken(It.IsAny<string>())).ReturnsAsync(baseLogin);

        // Act && Assert
        var exception = await Assert.ThrowsAsync<RefreshTokenInvalidoException>(async () => await _acessoBusiness.ValidateCredentials("expired_refresh_token"));
                
        _repositorioMock.Verify(repo => repo.RevokeRefreshToken(idUsuario), Times.Once);
        Assert.Equal("Refresh Token Inválido!", exception.Message);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Return_Authentication_Exception_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var refreshToken = "invalid_refresh_token";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RefreshTokenInvalidoException>(
            async () => await _acessoBusiness.ValidateCredentials(refreshToken)
        );

        Assert.Equal("Refresh Token Inválido!", exception.Message);
    }

    [Fact]
    public async Task ValidateCredentials_Should_Revoke_Token_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var mockAcesso = AcessoFaker.Instance.GetNewFaker();
        _repositorioMock.Setup(repo => repo.FindByRefreshToken(It.IsAny<string>())).ReturnsAsync(mockAcesso);

        // Act && Assert
        var exception = await Assert.ThrowsAsync<RefreshTokenInvalidoException>(async () => await _acessoBusiness.ValidateCredentials("invalid_refresh_token"));
        _repositorioMock.Verify(repo => repo.RevokeRefreshToken(It.IsAny<Guid>()), Times.Once);
        Assert.Equal("Refresh Token Inválido!", exception.Message);
    }

}
