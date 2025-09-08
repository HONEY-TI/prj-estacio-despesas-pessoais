using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using Despesas.Backend.Controllers;
using Despesas.GlobalException.CustomExceptions;
using Despesas.GlobalException.CustomExceptions.Core;
using Domain.Core.ValueObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public sealed class UsuarioControllerTest
{
    private Mock<IUsuarioBusiness<UsuarioDto>> _mockUsuarioBusiness;
    private UsuarioController _usuarioController;
    private List<UsuarioDto> _usuarioDtos;
        private UsuarioDto _usuarioNormal;
    private Mapper _mapper;

    public UsuarioControllerTest()
    {
        _mockUsuarioBusiness = new Mock<IUsuarioBusiness<UsuarioDto>>();
        _usuarioController = new UsuarioController(_mockUsuarioBusiness.Object);
        var usuarios = UsuarioFaker.Instance.GetNewFakersUsuarios(20);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<UsuarioProfile>(); }));
        _usuarioNormal = _mapper.Map<UsuarioDto>(usuarios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.User).First());
        _usuarioDtos = _mapper.Map<List<UsuarioDto>>(usuarios);
    }

    [Fact]
    public async Task Get_Returns_OkObjectResult_With_Usuario()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usauriosDtos = _mapper.Map<List<UsuarioDto>>(usaurios);
        Guid idUsuario = usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.User).Last().Id;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.FindById(It.IsAny<Guid>())).ReturnsAsync(usauriosDtos.Find(u => u.Id == idUsuario) ?? new());

        // Act
        var result = await _usuarioController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var usuarioDto = Assert.IsType<UsuarioDto>(result.Value);
        Assert.Equal(idUsuario, usuarioDto.Id);
        _mockUsuarioBusiness.Verify(bussines => bussines.FindById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Put_Should_Update_UsuarioDto()
    {
        // Arrange
        var usuarioDto = _usuarioDtos[4];
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).ReturnsAsync(usuarioDto);

        // Act
        var result = await _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact(Skip = "Disabled Implemntação  removida do código")]
    public async Task Put_Should_Returns_BadRequest_When_Telefone_IsNull()
    {
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        
        var usuarioDto = _mapper.Map<UsuarioDto>(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.Admin).First());
        usuarioDto.Telefone = null;
        var usauriosDtos = _mapper.Map<List<UsuarioDto>>(usaurios);
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Erro ao atualizar Usuário!"));

        // Act
        var result = await _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao atualizar Usuário!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.AtLeastOnce);
    }

    [Fact(Skip = "Disabled Implemntação  removida do código")]
    public async Task Put_Should_Returns_BadRequest_When_Email_IsNull()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = string.Empty;
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = await _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact(Skip = "Disabled Implemntação  removida do código")]
    public async Task Put_Should_Returns_BadRequest_When_Email_IsNullOrWhiteSpace()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = " ";
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = await _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact(Skip = "Disabled Implemntação  removida do código")]
    public async Task Put_Should_Returns_BadRequest_When_Email_IsInvalid()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = "invalidEmail.com";
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        //_mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Email inválido!"));

        // Act
        var result = await _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Email inválido!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public async Task Put_Should_Throw_CustomException_When_Usuario_NotFound()
    {
        // Arrange
        var usuarioDto = _usuarioNormal;
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Update(It.IsAny<UsuarioDto>())).ThrowsAsync(new CustomException("Erro ao atualizar dados pessoais do usuário!"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _usuarioController.Put(usuarioDto));
        Assert.Equal("Erro ao atualizar dados pessoais do usuário!", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public async Task Get_Should_Throw_CustomException_When_User_NotFound()
    {
        // Arrange
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.FindById(It.IsAny<Guid>())).ReturnsAsync((UsuarioDto?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UsuarioNaoEncontradoException>(() => _usuarioController.Get());
        Assert.Equal("Usuário não encontrado!", exception.Message);
        _mockUsuarioBusiness.Verify(b => b.FindById(It.IsAny<Guid>()), Times.Once);
    }


    [Fact]
    public async Task Post_Should_Create_UsuarioDto()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Create(It.IsAny<UsuarioDto>())).ReturnsAsync(usuarioDto);

        // Act
        var result = await _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Create(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public async Task Post_Should_Throw_CustomException_When_Service_Throws()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Create(It.IsAny<UsuarioDto>())).ThrowsAsync(new CustomException("Erro ao cadastrar Usuário!"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _usuarioController.Post(usuarioDto));
        Assert.Equal("Erro ao cadastrar Usuário!", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockUsuarioBusiness.Verify(b => b.Create(It.IsAny<UsuarioDto>()), Times.Once);
    }


    [Fact]
    public async Task Delete_Should_Return_Ok_When_Success()
    {
        var usuarioDto = _usuarioDtos.First();
        Usings.SetupBearerToken(usuarioDto.Id.Value, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Delete(It.IsAny<UsuarioDto>())).ReturnsAsync(true);

        var result = await _usuarioController.Delete(usuarioDto) as ObjectResult;

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)result.Value!);
    }

    [Fact]
    public async Task Delete_Should_Throw_CustomException_When_Delete_Fails()
    {
        // Arrange 
        var usuarioDto = _usuarioDtos.First();
        Usings.SetupBearerToken(usuarioDto.Id.Value, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Delete(It.IsAny<UsuarioDto>())).ThrowsAsync(new CustomException("Não foi possivél excluir este usuário."));

        // Act && Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _usuarioController.Delete(usuarioDto));
        Assert.Equal("Não foi possivél excluir este usuário.", exception.Message);
        Assert.Equal(400, exception.StatusCode);
        _mockUsuarioBusiness.Verify(b => b.Delete(It.IsAny<UsuarioDto>()), Times.Once);
    }


    [Fact]
    public async Task GetProfileImage_Should_Return_File_When_ImageExists()
    {
        var imageBytes = new byte[] { 1, 2, 3 };
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.GetProfileImage(It.IsAny<Guid>())).ReturnsAsync(imageBytes);

        var result = await _usuarioController.GetProfileImage();

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal(imageBytes, fileResult.FileContents);
    }

    [Fact]
    public async Task GetProfileImage_Should_Return_NoContent_When_Image_IsNull()
    {
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.GetProfileImage(It.IsAny<Guid>())).ReturnsAsync((byte[]?)null);

        var result = await _usuarioController.GetProfileImage();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutProfileImage_Should_Return_File_When_Success()
    {
        var fileMock = new Mock<IFormFile>();
        var content = new byte[] { 1, 2, 3 };
        var ms = new MemoryStream(content);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.ContentType).Returns("image/png");

        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.UpdateProfileImage(It.IsAny<Guid>(), It.IsAny<IFormFile>())).ReturnsAsync(content);

        var result = await _usuarioController.PutProfileImage(fileMock.Object);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal(content, fileResult.FileContents);
    }

    [Fact]
    public async Task PutProfileImage_Should_Return_NoContent_When_Image_IsEmpty()
    {
        var fileMock = new Mock<IFormFile>();
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.UpdateProfileImage(It.IsAny<Guid>(), It.IsAny<IFormFile>()))
            .Returns(Task.Run(() => (byte[]?)Array.Empty<byte>()));

        var result = await _usuarioController.PutProfileImage(fileMock.Object);

        Assert.IsType<NoContentResult>(result);
    }

}
