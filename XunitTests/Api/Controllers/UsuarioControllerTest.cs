using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using Despesas.Backend.Controllers;
using Domain.Core.ValueObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public sealed class UsuarioControllerTest
{
    private Mock<IUsuarioBusiness<UsuarioDto>> _mockUsuarioBusiness;
    private UsuarioController _usuarioController;
    private List<UsuarioDto> _usuarioDtos;
    private UsuarioDto administrador;
    private UsuarioDto usuarioNormal;
    private Mapper _mapper;

    public UsuarioControllerTest()
    {
        _mockUsuarioBusiness = new Mock<IUsuarioBusiness<UsuarioDto>>();
        _usuarioController = new UsuarioController(_mockUsuarioBusiness.Object);
        var usuarios = UsuarioFaker.Instance.GetNewFakersUsuarios(20);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<UsuarioProfile>(); }));
        administrador = _mapper.Map<UsuarioDto>(usuarios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.Admin).First());
        usuarioNormal = _mapper.Map<UsuarioDto>(usuarios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.User).First());
        _usuarioDtos = _mapper.Map<List<UsuarioDto>>(usuarios);
    }

    [Fact]
    public void Get_Returns_OkObjectResult_With_Usuario()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usauriosDtos = _mapper.Map<List<UsuarioDto>>(usaurios);
        Guid idUsuario = usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.User).Last().Id;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.FindById(It.IsAny<Guid>())).Returns(usauriosDtos.Find(u => u.Id == idUsuario) ?? new());

        // Act
        var result = _usuarioController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var usuarioDto = Assert.IsType<UsuarioDto>(result.Value);
        Assert.Equal(idUsuario, usuarioDto.Id);
        _mockUsuarioBusiness.Verify(bussines => bussines.FindById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void Put_Should_Update_UsuarioDto()
    {
        // Arrange
        var usuarioDto = _usuarioDtos[4];
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Returns(usuarioDto);

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Telefone_IsNull()
    {
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        
        var usuarioDto = _mapper.Map<UsuarioDto>(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.Perfil.Admin).First());
        usuarioDto.Telefone = null;
        var usauriosDtos = _mapper.Map<List<UsuarioDto>>(usaurios);
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Erro ao atualizar Usuário!"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao atualizar Usuário!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.AtLeastOnce);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Email_IsNull()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = string.Empty;
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Email_IsNullOrWhiteSpace()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = " ";
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Email_IsInvalid()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = "invalidEmail.com";
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Email inválido!"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Email inválido!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_OkObjectResult_with_Empty_Result_When_Usuario_IsNull()
    {
        // Arrange
        var usuarioDto = usuarioNormal;
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Usuário não encontrado!"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não encontrado!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void Get_Should_Returns_BadRequest_When_User_NotFound()
    {
        // Arrange
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.FindById(It.IsAny<Guid>())).Returns((UsuarioDto?)null);

        // Act
        var result = _usuarioController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Usuário não encontrado!", result.Value);
    }

    [Fact]
    public void Post_Should_Create_UsuarioDto()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Create(It.IsAny<UsuarioDto>())).Returns(usuarioDto);

        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Create(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_BadRequest_When_Exception()
    {
        var usuarioDto = _usuarioDtos.First();
        Guid idUsuario = usuarioDto.Id.Value;
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Create(It.IsAny<UsuarioDto>()))
            .Throws(new ArgumentException("Erro ao cadastrar Usuário!"));

        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro ao cadastrar Usuário!", result.Value);
    }

    [Fact]
    public void Delete_Should_Return_Ok_When_Success()
    {
        var usuarioDto = _usuarioDtos.First();
        Usings.SetupBearerToken(usuarioDto.Id.Value, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Delete(It.IsAny<UsuarioDto>())).Returns(true);

        var result = _usuarioController.Delete(usuarioDto) as ObjectResult;

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)result.Value!);
    }

    [Fact]
    public void Delete_Should_Return_BadRequest_When_Fails()
    {
        var usuarioDto = _usuarioDtos.First();
        Usings.SetupBearerToken(usuarioDto.Id.Value, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.Delete(It.IsAny<UsuarioDto>()))
            .Throws(new ArgumentException("Não foi possivél excluir este usuário."));

        var result = _usuarioController.Delete(usuarioDto) as ObjectResult;

        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Não foi possivél excluir este usuário.", result.Value);
    }

    [Fact]
    public void GetProfileImage_Should_Return_File_When_ImageExists()
    {
        var imageBytes = new byte[] { 1, 2, 3 };
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.GetProfileImage(It.IsAny<Guid>())).Returns(imageBytes);

        var result = _usuarioController.GetProfileImage();

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal(imageBytes, fileResult.FileContents);
    }

    [Fact]
    public void GetProfileImage_Should_Return_NoContent_When_Image_IsNull()
    {
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.GetProfileImage(It.IsAny<Guid>())).Returns((byte[]?)null);

        var result = _usuarioController.GetProfileImage();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void PutProfileImage_Should_Return_File_When_Success()
    {
        var fileMock = new Mock<IFormFile>();
        var content = new byte[] { 1, 2, 3 };
        var ms = new MemoryStream(content);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.ContentType).Returns("image/png");

        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.UpdateProfileImage(It.IsAny<Guid>(), It.IsAny<IFormFile>())).Returns(content);

        var result = _usuarioController.PutProfileImage(fileMock.Object);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal(content, fileResult.FileContents);
    }

    [Fact]
    public void PutProfileImage_Should_Return_NoContent_When_Image_IsEmpty()
    {
        var fileMock = new Mock<IFormFile>();
        Guid idUsuario = Guid.NewGuid();
        Usings.SetupBearerToken(idUsuario, _usuarioController);
        _mockUsuarioBusiness.Setup(b => b.UpdateProfileImage(It.IsAny<Guid>(), It.IsAny<IFormFile>()))
            .Returns((byte[]?)Array.Empty<byte>());

        var result = _usuarioController.PutProfileImage(fileMock.Object);

        Assert.IsType<NoContentResult>(result);
    }

}
