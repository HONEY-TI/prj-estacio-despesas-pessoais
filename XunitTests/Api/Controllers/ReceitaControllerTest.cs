using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using Despesas.Backend.Controllers;
using Despesas.GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public sealed class ReceitaControllerTest
{
    private Mock<IBusinessBase<ReceitaDto, Receita>> _mockReceitaBusiness;
    private ReceitaController _receitaController;
    private List<ReceitaDto> _receitaDtos;
    private Mapper _mapper;

    public ReceitaControllerTest()
    {
        _mockReceitaBusiness = new Mock<IBusinessBase<ReceitaDto, Receita>>();
        _receitaController = new ReceitaController(_mockReceitaBusiness.Object);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<ReceitaProfile>(); }));
        _receitaDtos = _mapper.Map<List<ReceitaDto>>(ReceitaFaker.Instance.Receitas());
    }

    [Fact(Skip = "Disabled Controller não dispara mais exception quando resultado é Null")]
    public async Task Get_Should_Returns_OkResults_With_Null_List_When_TryCatch_ThrowsError()
    {
        // Arrange
        Guid idUsuario = _receitaDtos.First().UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.FindAll(It.IsAny<Guid>())).Throws<Exception>();

        // Act
        var result = await _receitaController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(result.Value);
        Assert.IsType<List<ReceitaDto>>(result.Value);
        var lstReceita = result.Value as List<ReceitaDto>;
        Assert.NotNull(lstReceita);
        Assert.Empty(lstReceita);
        _mockReceitaBusiness.Verify(b => b.FindAll(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Get_Should_Return_All_Receitas_From_Usuario()
    {
        // Arrange
        Guid idUsuario = _receitaDtos.First().UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.FindAll(It.IsAny<Guid>())).ReturnsAsync(_receitaDtos);

        // Act
        var result = await _receitaController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(_receitaDtos, result.Value);
        _mockReceitaBusiness.Verify(b => b.FindAll(It.IsAny<Guid>()), Times.Once);
    }

    [Fact(Skip = "Disabled Controller não dispara mais exception quando resultado é Null")]
    public async Task GetById_Should_Returns_BadRequest_When_Receita_NULL()
    {
        // Arrange
        var receitaDto = _receitaDtos.First();
        var idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((ReceitaDto)null);

        // Act
        var result = await _receitaController.GetById(receitaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Nenhuma receita foi encontrada.", result!.Value);
        _mockReceitaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }


    [Fact]
    public async Task GetById_Should_Returns_OkResults_With_Despesas()
    {
        // Arrange
        var receita = _receitaDtos.Last();
        Guid idUsuario = receita.UsuarioId;
        var receitaId = receita.Id;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(receita);

        // Act
        var result = await _receitaController.GetById(receitaId.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var _receita = result.Value as ReceitaDto;
        Assert.NotNull(_receita);
        Assert.IsType<ReceitaDto>(_receita);
        _mockReceitaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetById_Should_Throw_CustomException_When_Receita_NotFound()
    {
        // Arrange
        var receitaDto = _receitaDtos.First();
        var idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((ReceitaDto?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _receitaController.GetById(receitaDto.Id.Value));
        Assert.Equal("Nenhuma receita foi encontrada.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockReceitaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }


    [Fact]
    public async Task Post_Should_Create_Receita()
    {
        // Arrange
        var receitaDto = _receitaDtos[3];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.Create(It.IsAny<ReceitaDto>())).ReturnsAsync(receitaDto);

        // Act
        var result = await _receitaController.Post(receitaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var _receita = result.Value as ReceitaDto;
        Assert.NotNull(_receita);
        Assert.IsType<ReceitaDto>(_receita);
        _mockReceitaBusiness.Verify(b => b.Create(It.IsAny<ReceitaDto>()), Times.Once());
    }

    [Fact]
    public async Task Post_Should_Throw_CustomException_When_Create_Fails()
    {
        // Arrange
        var receitaDto = _receitaDtos[3];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness
            .Setup(business => business.Create(It.IsAny<ReceitaDto>()))
            .ReturnsAsync((ReceitaDto?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _receitaController.Post(receitaDto));
        Assert.Equal("Não foi possível realizar o cadastro da receita!", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockReceitaBusiness.Verify(b => b.Create(It.IsAny<ReceitaDto>()), Times.Once);
    }


    [Fact]
    public async Task Put_Should_Update_Receita()
    {
        // Arrange
        var receitaDto = _receitaDtos[4];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.Update(It.IsAny<ReceitaDto>())).ReturnsAsync(receitaDto);

        // Act
        var result = await _receitaController.Put(receitaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var _receita = result.Value as ReceitaDto;
        Assert.NotNull(_receita);
        Assert.IsType<ReceitaDto>(_receita);
        _mockReceitaBusiness.Verify(b => b.Update(It.IsAny<ReceitaDto>()), Times.Once);
    }

    [Fact]
    public async Task Put_Should_Throw_CustomException_When_Update_Fails()
    {
        var receitaDto = _receitaDtos[3];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.Update(It.IsAny<ReceitaDto>())).ReturnsAsync((ReceitaDto?)null);

        var exception = await Assert.ThrowsAsync<CustomException>(() => _receitaController.Put(receitaDto));
        Assert.Equal("Não foi possível atualizar o cadastro da receita.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockReceitaBusiness.Verify(b => b.Update(It.IsAny<ReceitaDto>()), Times.Once);
    }


    [Fact]
    public async Task Delete_Should_Returns_OkResult()
    {
        // Arrange
        var receitaDto = _receitaDtos[2];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.Delete(It.IsAny<ReceitaDto>())).ReturnsAsync(true);
        _mockReceitaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(receitaDto);

        // Act
        var result = await _receitaController.Delete(receitaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);  
        _mockReceitaBusiness.Verify(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockReceitaBusiness.Verify(b => b.Delete(It.IsAny<ReceitaDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_With_InvalidToken_Returns_BadRequest()
    {
        // Arrange
        var receitaDto = _receitaDtos[2];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(Guid.Empty, _receitaController);
        _mockReceitaBusiness.Setup(business => business.Delete(It.IsAny<ReceitaDto>())).ReturnsAsync(false);
        _mockReceitaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((ReceitaDto?)null);

        // Act
        var result = await _receitaController.Delete(receitaDto.Id.Value);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro ao excluir Receita!", badRequestResult.Value);
        _mockReceitaBusiness.Verify(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockReceitaBusiness.Verify(b => b.Delete(It.IsAny<ReceitaDto>()), Times.Once);
    }


    [Fact]
    public async Task Delete_Should_Returns_BadResquest_When_Receita_Not_Deleted()
    {
        // Arrange
        var receitaDto = _receitaDtos[2];
        Guid idUsuario = receitaDto.UsuarioId;
        Usings.SetupBearerToken(idUsuario, _receitaController);
        _mockReceitaBusiness.Setup(business => business.Delete(It.IsAny<ReceitaDto>())).ReturnsAsync(false);
        _mockReceitaBusiness.Setup(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(receitaDto);

        // Act
        var result = await _receitaController.Delete(receitaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao excluir Receita!", message);
        _mockReceitaBusiness.Verify(business => business.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockReceitaBusiness.Verify(b => b.Delete(It.IsAny<ReceitaDto>()), Times.Once);
    }
}
