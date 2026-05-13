using Microsoft.AspNetCore.Mvc;
using __mock__.Entities;
using Despesas.Backend.Controllers;
using Despesas.Application.Abstractions;

namespace Api.Controllers;

public sealed class GraficosControllerTest
{
    private Mock<IGraficosBusiness> _mockGraficoBusiness;
    private GraficosController _GraficoController;

    public GraficosControllerTest()
    {
        _mockGraficoBusiness = new Mock<IGraficosBusiness>();
        _GraficoController = new GraficosController(_mockGraficoBusiness.Object);
    }

    [Fact]
    public async Task GetDadosGraficoPorAno_Should_Return_GraficoData()
    {
        // Arrange
        var dadosGrafico = GraficoFaker.GetNewFaker();
        var idUsuario = Guid.NewGuid();
        DateTime anoMes = DateTime.Today;
        Usings.SetupBearerToken(idUsuario, _GraficoController);
        _mockGraficoBusiness.Setup(business => business.GetDadosGraficoByAnoByIdUsuario(idUsuario, anoMes)).Returns(Task.Run(() => dadosGrafico));

        // Act
        var result = await _GraficoController.GetByAnoByIdUsuario(anoMes);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var graficoData = okResult.Value;
    }

    [Fact]
    public async Task GetDadosGraficoPorAno_Returns_BadRequest_When_Throws_Error()
    {
        // Arrange
        var dadosGrafico = GraficoFaker.GetNewFaker();
        DateTime anoMes = DateTime.Today;
        Usings.SetupBearerToken(Guid.Empty, _GraficoController);
        _mockGraficoBusiness.Setup(business => business.GetDadosGraficoByAnoByIdUsuario(It.IsAny<Guid>(), It.IsAny<DateTime>())).Throws(new Exception());

        // Act
        var result = await _GraficoController.GetByAnoByIdUsuario(anoMes) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro ao gerar dados do Gráfico!", result.Value);
    }
}