using Microsoft.AspNetCore.Mvc;
using __mock__.Entities;
using Despesas.Backend.Controllers;
using Despesas.Application.Dtos;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos.Abstractions;

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

    [Fact]
    public void Post_With_ValidData_Returns_OkResult()
    {
        // Arrange
        var acesso = AcessoFaker.Instance.GetNewFaker();
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM(acesso.Usuario);
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>()));

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);
    }

    [Fact]
    public void Post_With_ValidData_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws<Exception>();

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Não foi possível realizar o cadastro.", message);
    }

    [Fact]
    public void Post_With_Null_Telefone_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.Telefone = string.Empty;
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws(new ArgumentException("Campo Telefone não pode ser em branco"));

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Telefone não pode ser em branco", message);
    }

    [Fact]
    public void Post_With_NUll_Email_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));
        acessoDto.Email = string.Empty;

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
    }

    [Fact]
    public void Post_With_InvalidEmail_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws(new ArgumentException("Email inválido!"));
        acessoDto.Email = "email";

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Email inválido!", message);
    }

    [Fact]
    public void Post_With_NUll_Password_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.Senha = string.Empty;
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws(new ArgumentException("Campo Senha não pode ser em branco ou nulo"));
        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Senha não pode ser em branco ou nulo", message);
    }

    [Fact]
    public void Post_With_NUll_ConfirmedPassword_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws(new ArgumentException("Campo Confirma Senha não pode ser em branco ou nulo"));
        acessoDto.ConfirmaSenha = string.Empty;

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Confirma Senha não pode ser em branco ou nulo", message);
    }

    [Fact]
    public void Post_With_Password_Mismatch_Returns_BadRequest()
    {
        // Arrange
        var acessoDto = AcessoFaker.Instance.GetNewFakerVM();
        acessoDto.ConfirmaSenha = "senha Errada";
        _mockAcessoBusiness.Setup(b => b.Create(It.IsAny<AcessoDto>())).Throws(new ArgumentException("Senha e Confirma Senha são diferentes!"));

        // Act
        var result = _acessoController.Post(acessoDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Senha e Confirma Senha são diferentes!", message);
    }

    [Fact]
    public void SignIn_BadRequest_When_TryCatch_Throws_ArgumentException()
    {
        // Arrange
        var loginVM = new LoginDto { Email = "teste@teste.com", Senha = "password" };
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>())).Throws<ArgumentException>();

        // Act
        var result = _acessoController.SignIn(loginVM) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void SignIn_With_ValidData_Returns_ObjectResult()
    {
        // Arrange
        var loginVM = new LoginDto { Email = "teste@teste.com", Senha = "password" };
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<LoginDto>())).Returns(new BaseAuthenticationDto());

        // Act
        var result = _acessoController.SignIn(loginVM) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public void SignIn_With_InvalidEmail_Returns_BadRequest_EmailInvalido()
    {
        // Arrange
        var loginVM = new LoginDto { Email = "email@invalido.com", Senha = "password" };

        // Act
        var result = _acessoController.SignIn(loginVM) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void SignIn_With_InvalidEmail_Returns_BadRequest_Login_Erro()
    {
        // Arrange
        var loginVM = new LoginDto { Email = "email", Senha = "password" };

        // Act
        var result = _acessoController.SignIn(loginVM) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Não foi possível realizar o login do usuário.", message);
    }

    [Fact]
    public void ChangePassword_With_ValidData_Returns_OkResult()
    {
        // Arrange
        var changePasswordVM = new ChangePasswordDto { Senha = "!12345", ConfirmaSenha = "!12345" };
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);
        _mockAcessoBusiness.Setup(b => b.ChangePassword(It.IsAny<Guid>(), "!12345"));

        // Act
        var result = _acessoController.ChangePassword(changePasswordVM) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);
    }

    [Fact]
    public void ChangePassword_With_NULL_Password_Returns_BadRequest()
    {
        // Arrange
        var changePasswordVM = new ChangePasswordDto { Senha = null, ConfirmaSenha = "!12345" };
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);
        ChangePasswordDto? nullChangePasswordDto = null;

        // Act
        var result = _acessoController.ChangePassword(nullChangePasswordDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.", message);
    }

    [Fact]
    public void ChangePassword_With_NULL_ConfirmedPassword_Returns_BadRequest()
    {
        // Arrange
        var changePasswordVM = new ChangePasswordDto { Senha = "!12345", ConfirmaSenha = null };
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);
        ChangePasswordDto? nullChangePasswordDto = null;

        // Act
        var result = _acessoController.ChangePassword(nullChangePasswordDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.", message);
    }

    [Fact]
    public void ChangePassword_With_ValidData_Returns_BadRequest()
    {
        // Arrange
        var changePasswordVM = new ChangePasswordDto { Senha = "!12345", ConfirmaSenha = "!12345" };
        var idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _acessoController);
        _mockAcessoBusiness.Setup(b => b.ChangePassword(It.IsAny<Guid>(), "!12345")).Throws(new Exception());

        // Act
        var result = _acessoController.ChangePassword(changePasswordVM) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.", message);
    }

    [Fact]
    public void RecoveryPassword_WithValidEmail_ReturnsOkResult()
    {
        // Arrange
        var email = "teste@teste.com";
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>())).Callback(() => { });
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>()));
        Usings.SetupBearerToken(Guid.NewGuid(), _acessoController);

        // Act
        var result = _acessoController.RecoveryPassword(email) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);
    }

    [Fact]
    public void RecoveryPassword_With_NUll_Email_Returns_BadRequest()
    {
        // Arrange
        var email = string.Empty;
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>())).Throws<Exception>();
        // Act
        var result = _acessoController.RecoveryPassword(email) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void RecoveryPassword_Lenght_Bigger_Than_256_Email_Returns_BadRequest()
    {
        // Arrange
        var email = new string('A', 257);
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>())).Throws<Exception>();

        // Act
        var result = _acessoController.RecoveryPassword(email) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void RecoveryPassword_With_Invalid_Email_Returns_BadRequest()
    {
        // Arrange
        var email = "email Invalido";
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>())).Throws<Exception>();
        // Act
        var result = _acessoController.RecoveryPassword(email) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void RecoveryPassword_WithInvalid_Email_Returns_BadRequest_Email_Nao_Enviado()
    {
        // Arrange
        var email = "email@invalido.com";
        _mockAcessoBusiness.Setup(b => b.RecoveryPassword(It.IsAny<string>())).Throws<Exception>();

        // Act
        var result = _acessoController.RecoveryPassword(email) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Refresh_With_ValidData_Returns_OkResult()
    {
        // Arrange
        var authenticationDto = new BaseAuthenticationDto();
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<string>())).Returns(new BaseAuthenticationDto());

        // Act
        var result = _acessoController.Refresh("fakeRefreshToken") as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Refresh_With_InvalidData_Returns_BadRequest()
    {
        // Arrange
        var authenticationDto = new BaseAuthenticationDto();
        _acessoController.ModelState.AddModelError("Key", "Error");
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<string>())).Returns(() => null);
        // Act
        var result = _acessoController.Refresh("fakeRefreshToken") as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }

    [Fact]
    public void Refresh_With_Null_Result_Returns_BadRequest()
    {
        // Arrange
        var authenticationDto = new BaseAuthenticationDto();
        _mockAcessoBusiness.Setup(b => b.ValidateCredentials(It.IsAny<string>())).Returns(() => null);

        // Act
        var result = _acessoController.Refresh("fakeRefreshToken") as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }
}