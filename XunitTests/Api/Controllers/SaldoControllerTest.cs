using Microsoft.AspNetCore.Mvc;
using Despesas.Backend.Controllers;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;

namespace Api.Controllers;

public sealed class SaldoControllerTest
{
    private Mock<ISaldoBusiness> _mockSaldoBusiness;
    private SaldoController _SaldoController;
    public SaldoControllerTest()
    {
        _mockSaldoBusiness = new Mock<ISaldoBusiness>();
        _SaldoController = new SaldoController(_mockSaldoBusiness.Object);
    }

    [Fact]
    public async Task GetSaldo_Should_Return_Saldo()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _SaldoController);
        decimal saldo = 1000.99m;
        _mockSaldoBusiness.Setup(business => business.GetSaldo(idUsuario)).ReturnsAsync(saldo);

        // Act
        var result = await _SaldoController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSaldo = okResult.Value as SaldoDto;
        Assert.IsType<decimal>(returnedSaldo?.saldo);
        Assert.Equal(saldo, returnedSaldo.saldo);
    }

    [Fact]
    public async Task GetSaldo_Returns_BadRequest_When_Throws_Error()
    {
        // Arrange
        var idUsuario = Guid.Empty;
        Usings.SetupBearerToken(idUsuario, _SaldoController);
        _mockSaldoBusiness.Setup(business => business.GetSaldo(idUsuario)).Throws(new Exception());

        // Act
        var result = await _SaldoController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao gerar saldo!", message);
        _mockSaldoBusiness.Verify(b => b.GetSaldo(idUsuario), Times.Once);
    }

    [Fact]
    public async Task GetSaldoByAno_Should_Return_Saldo()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _SaldoController);
        decimal saldo = 897.99m;
        _mockSaldoBusiness.Setup(business => business.GetSaldoAnual(DateTime.Today, idUsuario)).ReturnsAsync(saldo);

        // Act
        var result = await _SaldoController.GetSaldoByAno(DateTime.Today) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSaldo = okResult.Value as SaldoDto;
        Assert.IsType<decimal>(returnedSaldo?.saldo);
        Assert.Equal(saldo, returnedSaldo.saldo);
    }

    [Fact]
    public async Task GetSaldoByAno_Returns_BadRequest_When_Throws_Error()
    {
        // Arrange
        var idUsuario = Guid.Empty;
        Usings.SetupBearerToken(idUsuario, _SaldoController);
        _mockSaldoBusiness.Setup(business => business.GetSaldoAnual(DateTime.Today, idUsuario)).Throws(new Exception());

        // Act
        var result = await _SaldoController.GetSaldoByAno(DateTime.Today) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao gerar saldo!", message);
        _mockSaldoBusiness.Verify(b => b.GetSaldoAnual(DateTime.Today, idUsuario), Times.Once);
    }

    [Fact]
    public async Task GetSaldoByMesAno_Should_Return_Saldo()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _SaldoController);
        decimal saldo = 178740.99m;
        _mockSaldoBusiness.Setup(business => business.GetSaldoByMesAno(DateTime.Today, idUsuario)).ReturnsAsync(saldo);

        // Act
        var result = await _SaldoController.GetSaldoByMesAno(DateTime.Today) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSaldo = okResult.Value as SaldoDto;
        Assert.IsType<decimal>(returnedSaldo?.saldo);
        Assert.Equal(saldo, returnedSaldo.saldo);
    }

    [Fact]
    public async Task GetSaldoByMesAno_Returns_BadRequest_When_Throws_Error()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _SaldoController);
        _mockSaldoBusiness.Setup(business => business.GetSaldoByMesAno(DateTime.Today, idUsuario)).Throws(new Exception());

        // Act
        var result = await _SaldoController.GetSaldoByMesAno(DateTime.Today) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao gerar saldo!", message);
        _mockSaldoBusiness.Verify(b => b.GetSaldoByMesAno(DateTime.Today, idUsuario), Times.Once);
    }
}
