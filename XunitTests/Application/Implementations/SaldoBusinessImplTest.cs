using Despesas.Application.Implementations;
using Repository.Persistency.Abstractions;
using System.Threading.Tasks;

namespace Application;
public sealed class SaldoBusinessImplTest
{
    private readonly Mock<ISaldoRepositorio> _repositorioMock;
    private readonly SaldoBusinessImpl _saldoBusiness;

    public SaldoBusinessImplTest()
    {
        _repositorioMock = new Mock<ISaldoRepositorio>();
        _saldoBusiness = new SaldoBusinessImpl(_repositorioMock.Object);
    }

    [Fact]
    public async Task GetSaldo_Should_Return_Saldo_As_Decimal()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var saldo = 100.50m;
        _repositorioMock.Setup(r => r.GetSaldo(idUsuario)).Returns(saldo);

        // Act
        var result = await _saldoBusiness.GetSaldo(idUsuario);

        // Assert
        Assert.Equal(saldo, result.saldo);
        _repositorioMock.Verify(r => r.GetSaldo(idUsuario), Times.Once);
    }

    [Fact]
    public async Task GetSaldoByAno_Should_Return_Saldo_As_Decimal()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var saldo = 300.33m;
        _repositorioMock.Setup(r => r.GetSaldoByAno(DateTime.Today, idUsuario)).Returns(saldo);

        // Act
        var result = await _saldoBusiness.GetSaldoAnual(DateTime.Today, idUsuario);

        // Assert
        Assert.Equal(saldo, result.saldo);
        _repositorioMock.Verify(r => r.GetSaldoByAno(DateTime.Today, idUsuario), Times.Once);
    }

    [Fact]
    public async Task GetSaldoByMesAno_Should_Return_Saldo_As_Decimal()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();
        var saldo = 222.22m;
        _repositorioMock.Setup(r => r.GetSaldoByMesAno(DateTime.Today, idUsuario)).Returns(saldo);

        // Act
        var result = await _saldoBusiness.GetSaldoByMesAno(DateTime.Today, idUsuario);

        // Assert
        Assert.Equal(saldo, result.saldo);
        _repositorioMock.Verify(r => r.GetSaldoByMesAno(DateTime.Today, idUsuario), Times.Once);
    }
}