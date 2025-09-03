using Microsoft.AspNetCore.Mvc;
using __mock__.Entities;
using Despesas.Backend.Controllers;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;

namespace Api.Controllers;

public sealed class LancamentoControllerTest
{
    private Mock<ILancamentoBusiness<LancamentoDto>> _mockLancamentoBusiness;
    private LancamentoController _lancamentoController;
    private List<LancamentoDto> _lancamentoDtos;

    public LancamentoControllerTest()
    {
        _mockLancamentoBusiness = new Mock<ILancamentoBusiness<LancamentoDto>>();
        _lancamentoController = new LancamentoController(_mockLancamentoBusiness.Object);
        _lancamentoDtos = LancamentoFaker.LancamentoDtos();
    }

    [Fact]
    public async Task Get_Should_Return_LancamentoDtos()
    {
        // Arrange
        var lancamentoDtos = _lancamentoDtos;
        Guid idUsuario = _lancamentoDtos.First().UsuarioId;
        DateTime anoMes = DateTime.Now;
        Usings.SetupBearerToken(idUsuario, _lancamentoController);
        _mockLancamentoBusiness.Setup(business => business.FindByMesAno(anoMes, idUsuario)).Returns(Task.Run(() => lancamentoDtos.FindAll(l => l.UsuarioId == idUsuario)));

        // Act
        var result = await _lancamentoController.Get(anoMes) as ObjectResult;

        // Assert
        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var lancamentos = result.Value as List<LancamentoDto>;
        Assert.NotNull(lancamentos);
        Assert.NotEmpty(lancamentos);
        var returnedLancamentoDtos = Assert.IsType<List<LancamentoDto>>(lancamentos);
        Assert.Equal(lancamentoDtos.FindAll(l => l.UsuarioId == idUsuario), returnedLancamentoDtos);
        _mockLancamentoBusiness.Verify(b => b.FindByMesAno(anoMes, idUsuario), Times.Once);
    }

    [Fact]
    public async Task Get_Returns_OkResult_With_Empty_List_When_Lancamento_IsNull()
    {
        // Arrange
        var lancamentoDtos = _lancamentoDtos;
        Guid idUsuario = _lancamentoDtos.First().UsuarioId;
        DateTime anoMes = DateTime.Now;
        Usings.SetupBearerToken(idUsuario, _lancamentoController);
        _mockLancamentoBusiness.Setup(business => business.FindByMesAno(anoMes, idUsuario)).Returns(() => null);

        // Act
        var result = await _lancamentoController.Get(anoMes) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var lancamentos = result.Value as List<LancamentoDto>;
        Assert.NotNull(lancamentos);
        Assert.Empty(lancamentos);
        _mockLancamentoBusiness.Verify(b => b.FindByMesAno(anoMes, idUsuario), Times.Once);
    }

    [Fact]
    public async Task Get_Returns_OkResult_With_Empty_List_When_Lancamento_List_Count0()
    {
        // Arrange
        var lancamentoDtos = _lancamentoDtos;
        Guid idUsuario = _lancamentoDtos.First().UsuarioId;
        DateTime anoMes = DateTime.Now;
        Usings.SetupBearerToken(idUsuario, _lancamentoController);
        _mockLancamentoBusiness.Setup(business => business.FindByMesAno(anoMes, idUsuario)).Returns(Task.Run(() => new List<LancamentoDto>()));

        // Act
        var result = await _lancamentoController.Get(anoMes) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var lancamentos = result.Value as List<LancamentoDto>;
        Assert.NotNull(lancamentos);
        Assert.Empty(lancamentos);
        _mockLancamentoBusiness.Verify(b => b.FindByMesAno(anoMes, idUsuario), Times.Once);
    }

    [Fact]
    public async Task Get_Returns_OkResults_With_Empty_List_When_Throws_Error()
    {
        // Arrange
        var lancamentoDtos = _lancamentoDtos;
        Guid idUsuario = _lancamentoDtos.First().UsuarioId;
        DateTime anoMes = DateTime.Now;
        Usings.SetupBearerToken(idUsuario, _lancamentoController);
        _mockLancamentoBusiness.Setup(business => business.FindByMesAno(anoMes, idUsuario)).Throws(new Exception());

        // Act
        var result = await _lancamentoController.Get(anoMes) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var lancamentos = result.Value as List<LancamentoDto>;
        Assert.NotNull(lancamentos);
        Assert.Empty(lancamentos);
        _mockLancamentoBusiness.Verify(b => b.FindByMesAno(anoMes, idUsuario), Times.Once);
    }
}
