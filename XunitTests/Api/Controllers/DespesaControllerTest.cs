using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Backend.Controllers;
using Despesas.GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Mvc;

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
        _mockDespesaBusiness.Setup(business => business.FindAll(It.IsAny<Guid>())).ReturnsAsync(new List<DespesaDto>());

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


    [Fact(Skip = "Disable ")]
    public async Task GetById_Should_Returns_BadRequest_When_Despesa_NULL()
    {
        // Arrange
        var despesaDto = DespesaFaker.Instance.DespesasVMs().First();
        Guid idUsuario = despesaDto.UsuarioId;
        despesaDto.Categoria = null;
        despesaDto.CategoriaId = null;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        //_mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((DespesaDto)null);

        // Act
        var result = await _despesaController.Get(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Nenhuma despesa foi encontrada.", message);
        _mockDespesaBusiness.Verify(b => b.FindById(despesaDto.Id.Value, idUsuario), Times.Once);
    }

    [Fact]
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
    public async Task GetById_Should_Throw_CustomException_When_Despesa_Not_Found()
    {
        // Arrange
        var despesaDto = DespesaFaker.Instance.DespesasVMs().First();
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);

        _mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((DespesaDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _despesaController.Get(despesaDto.Id.Value));
        Assert.Equal("Nenhuma despesa foi encontrada.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockDespesaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }


    [Fact]
    public async Task Post_Should_Create_Despesa()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[3];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Create(despesaDto)).ReturnsAsync(despesaDto);

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
    public async Task Post_Should_Throw_CustomException_When_Despesa_Not_Created()
    {
        // Arrange
        var despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = despesaDtos[3];
        Guid idUsuario = despesaDto.UsuarioId;

        Usings.SetupBearerToken(idUsuario, _despesaController);

        _mockDespesaBusiness.Setup(business => business.Create(It.IsAny<DespesaDto>())).ReturnsAsync((DespesaDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _despesaController.Post(despesaDto));
        Assert.Equal("Não foi possível realizar o cadastro da despesa.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockDespesaBusiness.Verify(b => b.Create(It.IsAny<DespesaDto>()), Times.Once);
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
    public async Task Put_Should_Throw_CustomException_When_Despesa_Not_Updated()
    {
        // Arrange
        var despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = despesaDtos[3];
        Guid idUsuario = despesaDto.UsuarioId;

        Usings.SetupBearerToken(idUsuario, _despesaController);

        _mockDespesaBusiness.Setup(business => business.Update(It.IsAny<DespesaDto>())).ReturnsAsync((DespesaDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _despesaController.Put(despesaDto));
        Assert.Equal("Não foi possível atualizar o cadastro da despesa.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockDespesaBusiness.Verify(b => b.Update(It.IsAny<DespesaDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Returns_OkResult()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[2];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Delete(It.IsAny<DespesaDto>())).ReturnsAsync(true);
        _mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(despesaDto);

        // Act
        var result = await _despesaController.Delete(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);
        _mockDespesaBusiness.Verify(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockDespesaBusiness.Verify(b => b.Delete(It.IsAny<DespesaDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete__With_InvalidToken_Returns_BadRequest()
    {
        // Arrange
        var _despesaDtos = DespesaFaker.Instance.DespesasVMs();
        var despesaDto = _despesaDtos[2];
        Guid idUsuario = despesaDto.UsuarioId;
        Usings.SetupBearerToken(Guid.Empty, _despesaController);
        _mockDespesaBusiness.Setup(business => business.Delete(It.IsAny<DespesaDto>())).ReturnsAsync(false);
        _mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(despesaDto);
        // Act
        var result = await _despesaController.Delete(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao excluir Despesa!", message);
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
        _mockDespesaBusiness.Setup(business => business.Delete(It.IsAny<DespesaDto>())).ReturnsAsync(false);
        _mockDespesaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((DespesaDto)null);

        // Act
        var result = await _despesaController.Delete(despesaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao excluir Despesa!", message);
        _mockDespesaBusiness.Verify(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockDespesaBusiness.Verify(b => b.Delete(It.IsAny<DespesaDto>()), Times.Once);
    }
}