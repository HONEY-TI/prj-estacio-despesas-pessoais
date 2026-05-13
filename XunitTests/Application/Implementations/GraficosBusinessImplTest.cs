using Repository.Persistency.Abstractions;
using __mock__.Entities;
using Despesas.Application.Implementations;

namespace Application;
public sealed class GraficosBusinessImplTest
{
    private readonly Mock<IGraficosRepositorio> _repositorioMock;
    private readonly GraficosBusinessImpl _graficosBusinessImpl;

    public GraficosBusinessImplTest()
    {
        _repositorioMock = new Mock<IGraficosRepositorio>();
        _graficosBusinessImpl = new GraficosBusinessImpl(_repositorioMock.Object);
    }

    [Fact]
    public async Task GetDadosGraficoByAnoByIdUsuario_Should_Return_Grafico()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var data = new DateTime(2023, 10, 1);
        var graficoData = GraficoFaker.GetNewFaker();
        _repositorioMock.Setup(r => r.GetDadosGraficoByAno(idUsuario, data)).ReturnsAsync(graficoData);

        // Act
        var result = await _graficosBusinessImpl.GetDadosGraficoByAnoByIdUsuario(idUsuario, data);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Grafico>(result);
        _repositorioMock.Verify(r => r.GetDadosGraficoByAno(idUsuario, data), Times.Once);
    }
}