using Microsoft.AspNetCore.Mvc;
using __mock__.Entities;
using AutoMapper;
using Despesas.Backend.Controllers;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;

namespace Api.Controllers;

public sealed class DespesaControllerTest
{
    private Mock<IBusinessBase<DespesaDto, Despesa>> _mockDespesaBusiness;
    private DespesaController _despesaController;
    private Mapper _mapper;

    public DespesaControllerTest()
    {
        _mockDespesaBusiness = new Mock<IBusinessBase<DespesaDto, Despesa>>();
        _despesaController = new DespesaController(_mockDespesaBusiness.Object);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<DespesaProfile>(); }));
    }

    [Fact]
    public async Task Get_Should_Return_All_Despesas_From_Usuario()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        Guid idUsuario = _despesaDtos.First().UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.FindAll(idUsuario)).Returns(Task.Run(() => _despesaDtos));

        // Act
        var result = await _despesaController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(_despesaDtos, result.Value);
        _mockDespesaBusiness.Verify(b => b.FindAll(idUsuario), Times.Once);
    }

    [Fact]
    public async Task Get_Should_Returns_OkResults_With_Null_List_When_TryCatch_ThrowsError()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        Guid idUsuario = _despesaDtos.First().UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.FindAll(idUsuario)).Throws<Exception>();

        // Act
        var result = await _despesaController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(result.Value);
        Assert.IsType<List<DespesaDto>>(result.Value);
        var lstDespesas = result.Value as List<DespesaDto>;
        Assert.NotNull(lstDespesas);
        Assert.Empty(lstDespesas);
        _mockDespesaBusiness.Verify(b => b.FindAll(idUsuario), Times.Once);
    }


    [Fact]
    public async Task GetById_Should_Returns_BadRequest_When_Despesa_NULL()
    {
        // Arrange
        var despesaDto = DespesaFaker.Instance.DespesasVMs().First();
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(() => null);

        // Act
        var result = await _despesaController.Get(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Nenhuma despesa foi encontrada.", message);
        _mockDespesaBusiness.Verify(b => b.FindById(despesaDto.Id.Value, idUsuario), Times.Once);
    }

    [Fact(Skip = "Teste Unitário com Erro de implementação ")]
    public async Task GetById_Should_Returns_OkResults_With_Despesas()
    {
        // Arrange
        var despesaDto = DespesaFaker.Instance.GetNewFakerVM(Guid.NewGuid(), Guid.NewGuid());
        Guid idUsuario = despesaDto.UsuarioId;
        var despesaId = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(despesaDto);

        // Act
        var result = await _despesaController.Get(despesaId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var _despesa = result.Value;
        Assert.NotNull(_despesa);
        Assert.IsType<DespesaDto>(_despesa);
        _mockDespesaBusiness.Verify(b => b.FindById(despesaId, idUsuario), Times.Once);
    }

    [Fact]
    public async Task GetById_Should_Returns_BadRequest_When_Throws_Error()
    {
        // Arrange
        var despesaDto = DespesaFaker.Instance.DespesasVMs().First();
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.FindById(despesaDto.Id.Value, idUsuario)).Throws(new Exception());

        // Act
        var result = await _despesaController.Get(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Não foi possível realizar a consulta da despesa.", message);
        _mockDespesaBusiness.Verify(b => b.FindById(despesaDto.Id.Value, idUsuario), Times.Once);
    }

    [Fact]
    public async Task Post_Should_Create_Despesa()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[3];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Create(despesaDto)).Returns(Task.Run(() => despesaDto));

        // Act
        var result = await _despesaController.Post(despesaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var _despesa = result.Value;
        Assert.NotNull(_despesa);
        Assert.IsType<DespesaDto>(_despesa);
        _mockDespesaBusiness.Verify(b => b.Create(despesaDto), Times.Once());
    }

    [Fact]
    public async Task Post_Should_Returns_BadRequest_When_Throws_Error()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[3];
        Guid idUsuario = despesaDto.UsuarioId;

        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Create(despesaDto)).Throws(new Exception());

        // Act
        var result = await _despesaController.Post(despesaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Não foi possível realizar o cadastro da despesa.", message);
        _mockDespesaBusiness.Verify(b => b.Create(despesaDto), Times.Once);
    }

    [Fact]
    public async Task Put_Should_Update_Despesa()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[4];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Update(despesaDto)).Returns(Task.Run(() => despesaDto));

        // Act
        var result = await _despesaController.Put(despesaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var _despesa = result.Value;
        Assert.NotNull(_despesa);
        Assert.IsType<DespesaDto>(_despesa);
        _mockDespesaBusiness.Verify(b => b.Update(despesaDto), Times.Once);
    }

    [Fact]
    public async Task Put_Should_Returns_BadRequest_When_Despesa_Return_Null()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[3];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Update(despesaDto)).Returns(() => null);

        // Act
        var result = await _despesaController.Put(despesaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Não foi possível atualizar o cadastro da despesa.", message);
        _mockDespesaBusiness.Verify(b => b.Update(despesaDto), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Returns_OkResult()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[2];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Delete(despesaDto)).Returns(Task.Run(() => true));
        _mockDespesaBusiness.Setup(business => business.FindById(despesaDto.Id.Value, idUsuario)).Returns(Task.Run(() => despesaDto));

        // Act
        var result = await _despesaController.Delete(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);
        _mockDespesaBusiness.Verify(business => business.FindById(despesaDto.Id.Value, idUsuario), Times.Once);
        _mockDespesaBusiness.Verify(b => b.Delete(despesaDto), Times.Once);
    }

    [Fact]
    public async Task Delete__With_InvalidToken_Returns_BadRequest()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[2];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(Guid.Empty, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Delete(despesaDto)).Returns(Task.Run(() => true));
        _mockDespesaBusiness.Setup(business => business.FindById(despesaDto.Id.Value, idUsuario)).Returns(Task.Run(() => despesaDto));

        // Act
        var result = await _despesaController.Delete(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não permitido a realizar operação!", message);
        _mockDespesaBusiness.Verify(business => business.FindById(despesaDto.Id.Value, idUsuario), Times.Never);
        _mockDespesaBusiness.Verify(b => b.Delete(despesaDto), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Returns_BadResquest_When_Despesa_Not_Deleted()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[2];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Delete(despesaDto)).Returns(Task.Run(() => false));
        _mockDespesaBusiness.Setup(business => business.FindById(despesaDto.Id.Value, idUsuario)).Returns(Task.Run(() => despesaDto));

        // Act
        var result = await _despesaController.Delete(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao excluir Despesa!", message);
        _mockDespesaBusiness.Verify(business => business.FindById(despesaDto.Id.Value, idUsuario), Times.Once);
        _mockDespesaBusiness.Verify(b => b.Delete(despesaDto), Times.Once);
    }
}