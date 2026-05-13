using __mock__.Entities;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Abstractions;
using Despesas.Application.Dtos.Core;
using Despesas.Backend.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Despesas.GlobalException.CustomExceptions.Acesso;
using Despesas.GlobalException.CustomExceptions.Core;

namespace Api.Controllers;

public sealed class AcessoControllerTest
{
    private readonly Mock<IAcessoBusiness<AcessoDto, LoginDto>> _mockAcessoBusiness;
    private readonly AcessoController _acessoController;

    public AcessoControllerTest()
    {
        _mockAcessoBusiness = new Mock<IAcessoBusiness<AcessoDto, LoginDto>>();
        _acessoController = new AcessoController(_mockAcessoBusiness.Object);
    }

    // ----- POST -----
    [Fact]
    public async Task Post_With_ValidData_Returns_Ok()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Returns(Task.CompletedTask);

        var result = await _acessoController.Post(acessoDto) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.True((bool)result.Value.ToBoolean());
    }

    [Fact]
    public async Task Post_With_Business_Exception_Returns_BadRequest()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new CustomException("Não foi possível realizar o cadastro.", 400));

        var result = await Assert.ThrowsAsync<CustomException>(() => _acessoController.Post(acessoDto));
        Assert.Equal("Não foi possível realizar o cadastro.", result.Message);
    }

    [Fact]
    public async Task Post_With_Telefone_Null_Throws_CampoObrigatorio()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.Telefone = string.Empty;
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new CampoObrigatorioException("Telefone"));

        var ex = await Assert.ThrowsAsync<CampoObrigatorioException>(() => _acessoController.Post(acessoDto));
        Assert.Equal("Campo Telefone não pode ser em branco ou nulo!", ex.Message);
    }

    [Fact]
    public async Task Post_With_Email_Null_Throws_CampoObrigatorio()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.Email = string.Empty;
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new CampoObrigatorioException("Email"));

        var ex = await Assert.ThrowsAsync<CampoObrigatorioException>(() => _acessoController.Post(acessoDto));
        Assert.Equal("Campo Email não pode ser em branco ou nulo!", ex.Message);
    }

    [Fact]
    public async Task Post_With_InvalidEmail_Throws_EmailInvalidoException()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.Email = "email";
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new EmailInvalidoException());

        await Assert.ThrowsAsync<EmailInvalidoException>(() => _acessoController.Post(acessoDto));
    }

    [Fact]
    public async Task Post_With_Senha_Null_Throws_CampoObrigatorio()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.Senha = string.Empty;
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new CampoObrigatorioException("Senha"));

        var ex = await Assert.ThrowsAsync<CampoObrigatorioException>(() => _acessoController.Post(acessoDto));
        Assert.Equal("Campo Senha não pode ser em branco ou nulo!", ex.Message);
    }

    [Fact]
    public async Task Post_With_ConfirmaSenha_Null_Throws_CampoObrigatorio()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.ConfirmaSenha = string.Empty;
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new ConfirmaSenhaInvalidaException());

        var ex = await Assert.ThrowsAsync<ConfirmaSenhaInvalidaException>(() => _acessoController.Post(acessoDto));
        Assert.Equal("Campo Confirma Senha não pode ser em branco ou nulo!", ex.Message);
    }

    [Fact]
    public async Task Post_With_PasswordMismatch_Throws_AcessoException()
    {
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.ConfirmaSenha = "senhaErrada";
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()))
            .ThrowsAsync(new CustomException("Senha e Confirma Senha são diferentes!", 400));

        var ex = await Assert.ThrowsAsync<CustomException>(() => _acessoController.Post(acessoDto));
        Assert.Equal("Senha e Confirma Senha são diferentes!", ex.Message);
    }

    // ----- SIGN IN -----
    [Fact]
    public async Task SignIn_With_ValidData_Returns_Ok()
    {
        var loginDto = new LoginDto { Email = "teste@teste.com", Senha = "password" };
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>()))
            .ReturnsAsync(new AuthenticationDto { Authenticated = true });

        var result = await _acessoController.SignIn(loginDto) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task SignIn_With_InvalidCredentials_Throws_UsuarioInexistenteException()
    {
        var loginDto = new LoginDto { Email = "email@invalido.com", Senha = "password" };
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>()))
            .ThrowsAsync(new UsuarioInexistenteException());

        await Assert.ThrowsAsync<UsuarioInexistenteException>(() => _acessoController.SignIn(loginDto));
    }

    [Fact]
    public async Task SignIn_With_InvalidEmailFormat_Throws_EmailInvalidoException()
    {
        var loginDto = new LoginDto { Email = "emailinvalido", Senha = "password" };
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>()))
            .ThrowsAsync(new EmailInvalidoException());

        await Assert.ThrowsAsync<EmailInvalidoException>(() => _acessoController.SignIn(loginDto));
    }

    [Fact]
    public async Task SignIn_With_InvalidPassword_Throws_SenhaInvalidaException()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "teste@teste.com", Senha = "senhaErrada" };

        // Simula que o usuário existe, mas a senha não confere
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>()))
            .ThrowsAsync(new SenhaInvalidaException());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<SenhaInvalidaException>(() => _acessoController.SignIn(loginDto));
        Assert.Equal("Senha inválida!", ex.Message);
    }

    // ----- CHANGE PASSWORD -----
    [Fact]
    public async Task ChangePassword_With_ValidData_Returns_Ok()
    {
        var changePasswordDto = new ChangePasswordDto { Senha = "!12345", ConfirmaSenha = "!12345" };
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);
        _mockAcessoBusiness.Setup(b => b.ChangePassword(It.IsAny<Guid>(), "!12345"))
            .Returns(Task.CompletedTask);

        var result = await _acessoController.ChangePassword(changePasswordDto) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.True((bool)result.Value.ToBoolean());
    }

    [Fact]
    public async Task ChangePassword_With_Null_Throws_BadRequest()
    {
        ChangePasswordDto? dto = null;
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);

        var ex = await Assert.ThrowsAsync<TrocaSenhaException>(async () => await _acessoController.ChangePassword(dto));
        Assert.Equal("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.", ex.Message);
    }

    // ----- RECOVERY PASSWORD -----
    [Fact]
    public async Task RecoveryPassword_With_ValidEmail_Returns_Ok()
    {
        var email = "teste@teste.com";
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);

        var result = await _acessoController.RecoveryPassword(email) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.True((bool)result.Value.ToBoolean());
    }

    [Fact]
    public async Task RecoveryPassword_With_InvalidEmail_Throws_Exception()
    {
        var email = "emailinvalido@teste.com";
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>()))
            .ThrowsAsync(new UsuarioInexistenteException());

        await Assert.ThrowsAsync<UsuarioInexistenteException>(() => _acessoController.RecoveryPassword(email));
    }

    // ----- REFRESH -----
    [Fact]
    public async Task Refresh_With_ValidToken_Returns_Ok()
    {
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<string>()))
            .ReturnsAsync(new AuthenticationDto());

        var result = await _acessoController.Refresh("fakeToken") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task Refresh_With_InvalidToken_Throws_AcessoException()
    {
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<string>()))
            .ThrowsAsync(new CustomException("Token inválido", 400));

        var ex = await Assert.ThrowsAsync<CustomException>(() => _acessoController.Refresh("fakeToken"));
        Assert.Equal("Token inválido", ex.Message);
    }
}
