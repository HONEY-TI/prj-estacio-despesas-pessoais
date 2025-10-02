using __mock__.Entities;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Core;
using Despesas.Backend.Controllers;
using Despesas.GlobalException.CustomExceptions.Core;
using Domain.Core.ValueObject;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public sealed class CategoriaControllerTest
{
    private Mock<ICategoriaBusiness<CategoriaDto, Categoria>> _mockCategoriaBusiness;
    private CategoriaController _categoriaController;

    public CategoriaControllerTest()
    {
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
    }

    [Fact]
    public async Task Get_Returns_Ok_Result()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDtos = CategoriaFaker.Instance.CategoriasVMs();
        var UsuarioId = categoriaDtos.First().UsuarioId;
        Usings.SetupBearerToken(UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.FindAll(It.IsAny<Guid>())).ReturnsAsync(categoriaDtos.FindAll(c => c.UsuarioId == UsuarioId));

        // Act
        var result = await _categoriaController.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<CategoriaDto>>(result.Value);
        Assert.Equal(categoriaDtos.FindAll(c => c.UsuarioId == UsuarioId), result.Value);
    }


    [Fact]
    public async Task GetById_Returns_Ok_Result()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();
        var idCategoria = categoriaDto.UsuarioId;
        Usings.SetupBearerToken(idCategoria, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.GetById(idCategoria) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CategoriaDto>(result.Value);
        Assert.Equal(categoriaDto, result.Value);
    }

    [Fact]
    public async Task GetById_Returns_Ok_Result_Null()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();

        var idCategoria = categoriaDto.UsuarioId;

        Usings.SetupBearerToken(idCategoria, _categoriaController);

        _mockCategoriaBusiness.Setup(b => b.FindById(It.IsAny<Guid>())).ReturnsAsync((CategoriaDto)null);

        // Act
        var result = await _categoriaController.GetById(idCategoria) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Value as CategoriaDto);
    }

    [Fact]
    public async Task GetByTipoCategoria_Returns_Ok_Result_TipoCategoria_Todas()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var listCategoriaDto = CategoriaFaker.Instance.CategoriasVMs();

        var UsuarioId = listCategoriaDto.First().Id;

        Usings.SetupBearerToken(UsuarioId.Value, _categoriaController);
        var tipoCategoria = TipoCategoriaDto.Todas;
        _mockCategoriaBusiness.Setup(b => b.FindByTipocategoria(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(listCategoriaDto);

        // Act
        var result = await _categoriaController.GetByTipoCategoria(tipoCategoria) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<CategoriaDto>>(result.Value);
    }

    [Fact]
    public async Task GetByTipoCategoria_Returns_Ok_Result()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        List<CategoriaDto> listCategoriaDto = CategoriaFaker.Instance.CategoriasVMs();
        var UsuarioId = listCategoriaDto.First().Id;
        Usings.SetupBearerToken(UsuarioId.Value, _categoriaController);
        var tipoCategoria = TipoCategoriaDto.Despesa;
        _mockCategoriaBusiness.Setup(b => b.FindByTipocategoria(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(listCategoriaDto);

        // Act
        var result = await _categoriaController.GetByTipoCategoria(tipoCategoria) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<CategoriaDto>>(result.Value);
    }

    [Fact]
    public async Task Post_Returns_Ok_Result()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var obj = CategoriaFaker.Instance.CategoriasVMs().First();
        var categoriaDto = new CategoriaDto
        {
            Id = obj.Id,
            Descricao = obj.Descricao,
            UsuarioId = Guid.NewGuid(),
            IdTipoCategoria = (TipoCategoriaDto)TipoCategoria.CategoriaType.Despesa
        };
        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Create(It.IsAny<CategoriaDto>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Post(categoriaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(result.Value);
        Assert.IsType<CategoriaDto>(result.Value);
    }

    [Fact]
    public async Task Post_Returns_Bad_Request_When_TipoCategoria_Todas()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var obj = CategoriaFaker.Instance.CategoriasVMs().First();
        var categoriaDto = new CategoriaDto
        {
            Id = obj.Id,
            Descricao = obj.Descricao,
            UsuarioId = obj.UsuarioId,
            IdTipoCategoria = (int)TipoCategoriaDto.Todas
        };

        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Create(It.IsAny<CategoriaDto>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Post(categoriaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Nenhum tipo de Categoria foi selecionado!", message);
    }

    [Fact]
    public async Task Post_Returns_Bad_Request_When_Create_Returns_Null()
    {
        // Arrange 
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();
        categoriaDto.IdTipoCategoria = (TipoCategoriaDto)TipoCategoria.CategoriaType.Receita;
        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(async () => await _categoriaController.Post(categoriaDto));
        Assert.Equal("Não foi possível realizar o cadastro de uma nova categoria, tente mais tarde ou entre em contato com o suporte.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockCategoriaBusiness.Verify(b => b.Create(It.IsAny<CategoriaDto>()), Times.Once);
    }

    [Fact]
    public async Task Put_Returns_Ok_Result()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var obj = CategoriaFaker.Instance.CategoriasVMs().First();
        var categoriaDto = new CategoriaDto
        {
            Id = obj.Id,
            Descricao = obj.Descricao,
            UsuarioId = obj.UsuarioId,
            IdTipoCategoria = (TipoCategoriaDto)TipoCategoria.CategoriaType.Despesa
        };
        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Update(It.IsAny<CategoriaDto>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Put(categoriaDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CategoriaDto>(result.Value);
        Assert.Equal(result.Value, categoriaDto);
    }

    [Fact]
    public async Task Put_Returns_Bad_Request_TipoCategoria_Todas()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();
        categoriaDto.IdTipoCategoria = TipoCategoriaDto.Todas;
        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Update(It.IsAny<CategoriaDto>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Put(categoriaDto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Nenhum tipo de Categoria foi selecionado!", message);
    }

    [Fact]
    public async Task Put_Returns_Bad_Request_Categoria_Null()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();
        categoriaDto.IdTipoCategoria = (TipoCategoriaDto)1;
        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Update(It.IsAny<CategoriaDto>())).ReturnsAsync((CategoriaDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(async () => await _categoriaController.Put(categoriaDto));
        Assert.Equal("Erro ao atualizar categoria!", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockCategoriaBusiness.Verify(b => b.Update(It.IsAny<CategoriaDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Returns_Ok_Result()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var obj = CategoriaFaker.Instance.CategoriasVMs().Last();
        var idUsuario = Guid.NewGuid();
        var categoriaDto = new CategoriaDto
        {
            Id = obj.Id,
            Descricao = obj.Descricao,
            UsuarioId = idUsuario,
            IdTipoCategoria = (TipoCategoriaDto)TipoCategoria.CategoriaType.Receita
        };
        Usings.SetupBearerToken(idUsuario, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Delete(It.IsAny<CategoriaDto>())).ReturnsAsync(true);
        _mockCategoriaBusiness.Setup(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Delete(categoriaDto.Id.Value) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var message = (bool?)result.Value;
        Assert.True(message);
        _mockCategoriaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockCategoriaBusiness.Verify(b => b.Delete(It.IsAny<CategoriaDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Returns_BadRequest()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();
        Usings.SetupBearerToken(Guid.Empty, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.Delete(It.IsAny<CategoriaDto>())).ReturnsAsync(false);
        _mockCategoriaBusiness.Setup(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Delete(categoriaDto.Id.Value) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        _mockCategoriaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockCategoriaBusiness.Verify(b => b.Delete(It.IsAny<CategoriaDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Returns_BadRequest_When_TryCatch_ThrowError()
    {
        // Arrange
        _mockCategoriaBusiness = new Mock<ICategoriaBusiness<CategoriaDto, Categoria>>();
        _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        var categoriaDto = CategoriaFaker.Instance.CategoriasVMs().First();
        Usings.SetupBearerToken(categoriaDto.UsuarioId, _categoriaController);
        _mockCategoriaBusiness.Setup(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(categoriaDto);

        // Act
        var result = await _categoriaController.Delete(categoriaDto.Id.Value) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        _mockCategoriaBusiness.Verify(b => b.FindById(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mockCategoriaBusiness.Verify(b => b.Delete(It.IsAny<CategoriaDto>()), Times.Once);
    }
}