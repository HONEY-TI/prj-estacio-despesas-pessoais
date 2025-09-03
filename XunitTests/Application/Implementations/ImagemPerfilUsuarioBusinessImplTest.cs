using AutoMapper;
using Despesas.Infrastructure.Amazon.Abstractions;
using __mock__.Entities;
using Repository.Persistency.Generic;
using Despesas.Application.Implementations;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using System.Linq.Expressions;

namespace Application;
public sealed class ImagemPerfilUsuarioBusinessImplTests
{
    private readonly Mock<IRepositorio<ImagemPerfilUsuario>> _repositorioMock;
    private readonly Mock<IRepositorio<Usuario>> _repositorioUsuarioMock;
    private readonly ImagemPerfilUsuarioBusinessImpl<ImagemPerfilDto, UsuarioDto> _imagemPerfilUsuarioBusiness;
    private readonly Mock<IAmazonS3Bucket> _mockAmazonS3Bucket;
    private readonly List<ImagemPerfilUsuario> _imagensPerfil;
    private readonly IMapper _mapper;

    public ImagemPerfilUsuarioBusinessImplTests()
    {
        _imagensPerfil = ImagemPerfilUsuarioFaker.Instance.ImagensPerfilUsuarios();

        _repositorioMock = new Mock<IRepositorio<ImagemPerfilUsuario>>();
        _repositorioUsuarioMock = new Mock<IRepositorio<Usuario>>();
        _mockAmazonS3Bucket = new Mock<IAmazonS3Bucket>();

        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ImagemPerfilUsuarioProfile>()));

        _imagemPerfilUsuarioBusiness = new ImagemPerfilUsuarioBusinessImpl<ImagemPerfilDto, UsuarioDto>(
            _mapper,
            _repositorioMock.Object,
            _repositorioUsuarioMock.Object,
            _mockAmazonS3Bucket.Object
        );
    }

    [Fact]
    public async Task Create_Should_Returns_ImagemPerfilUsuarioDto()
    {
        // Arrange
        var usuario = UsuarioFaker.Instance.GetNewFaker();
        var imagemPerfil = ImagemPerfilUsuarioFaker.Instance.GetNewFaker(usuario);
        var imagemPerfilDto = _mapper.Map<ImagemPerfilDto>(imagemPerfil);

        _mockAmazonS3Bucket
            .Setup(x => x.WritingAnObjectAsync(It.IsAny<ImagemPerfilUsuario>(), It.IsAny<byte[]>()))
            .ReturnsAsync("http://teste.url");

        _repositorioUsuarioMock
            .Setup(x => x.Get(It.IsAny<Guid>()))
            .Returns(usuario);

        // Act
        var result = await _imagemPerfilUsuarioBusiness.Create(imagemPerfilDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ImagemPerfilDto>(result);
    }

    [Fact]
    public async Task Create_Should_Throw_ArgumentException_When_BucketFails()
    {
        // Arrange
        var usuario = UsuarioFaker.Instance.GetNewFaker();
        var imagemPerfil = ImagemPerfilUsuarioFaker.Instance.GetNewFaker(usuario);
        var imagemPerfilDto = _mapper.Map<ImagemPerfilDto>(imagemPerfil);

        _mockAmazonS3Bucket
            .Setup(x => x.WritingAnObjectAsync(It.IsAny<ImagemPerfilUsuario>(), It.IsAny<byte[]>()))
            .ThrowsAsync(new Exception());

        _repositorioUsuarioMock
            .Setup(x => x.Get(It.IsAny<Guid>()))
            .Returns(usuario);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _imagemPerfilUsuarioBusiness.Create(imagemPerfilDto));
    }

    [Fact]
    public async Task FindAll_Should_Return_List_Of_ImagemPerfilUsuarioDto()
    {
        // Arrange
        _repositorioMock.Setup(x => x.GetAll()).Returns(_imagensPerfil);

        // Act
        var result = await _imagemPerfilUsuarioBusiness.FindAll(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<ImagemPerfilDto>>(result);
        Assert.Equal(_imagensPerfil.Count, result.Count);
    }

    [Fact]
    public async Task FindById_Should_Returns_ImagemPerfilUsuarioDto()
    {
        // Arrange
        var imagemPerfil = _imagensPerfil.First();
        _repositorioMock.Setup(x => x.Get(imagemPerfil.Id)).Returns(imagemPerfil);

        // Act
        var result = await _imagemPerfilUsuarioBusiness.FindById(imagemPerfil.Id, imagemPerfil.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ImagemPerfilDto>(result);
    }

    [Fact]
    public async Task FindById_Should_Returns_Null_When_UsuarioId_Different()
    {
        // Arrange
        var imagemPerfil = _imagensPerfil.First();
        _repositorioMock.Setup(x => x.Get(imagemPerfil.Id)).Returns(imagemPerfil);

        // Act
        var result = await _imagemPerfilUsuarioBusiness.FindById(imagemPerfil.Id, Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Update_Should_Return_ImagemPerfilUsuarioDto()
    {
        // Arrange
        var imagemPerfil = _imagensPerfil.First();
        _repositorioMock.Setup(x => x.GetAll()).Returns(_imagensPerfil);
        _mockAmazonS3Bucket.Setup(x => x.WritingAnObjectAsync(It.IsAny<ImagemPerfilUsuario>(), It.IsAny<byte[]>())).ReturnsAsync("http://teste.url");
        _mockAmazonS3Bucket.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<ImagemPerfilUsuario>())).ReturnsAsync(true);

        var dto = _mapper.Map<ImagemPerfilDto>(imagemPerfil);

        // Act
        var result = await _imagemPerfilUsuarioBusiness.Update(dto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ImagemPerfilDto>(result);
    }

    [Fact]
    public async Task Update_Should_Throw_ArgumentException_When_ImagemPerfilNotFound()
    {
        // Arrange
        var dto = _mapper.Map<ImagemPerfilDto>(_imagensPerfil.First());
        _repositorioMock.Setup(x => x.GetAll()).Returns(new List<ImagemPerfilUsuario>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _imagemPerfilUsuarioBusiness.Update(dto));
    }

    [Fact]
    public async Task Delete_Should_Return_True_When_Successful()
    {
        // Arrange
        var imagemPerfil = _imagensPerfil.First();
        _repositorioMock.Setup(x => x.GetAll()).Returns(_imagensPerfil);
        _mockAmazonS3Bucket.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<ImagemPerfilUsuario>())).ReturnsAsync(true);
        _repositorioMock.Setup(x => x.Delete(It.IsAny<ImagemPerfilUsuario>()));
        _repositorioMock.Setup(x => x.Find(It.IsAny<Expression<Func<ImagemPerfilUsuario, bool>>>())).Returns(_imagensPerfil.Where(p => p.UsuarioId == imagemPerfil.UsuarioId));

        // Act
        var result = await _imagemPerfilUsuarioBusiness.Delete(imagemPerfil.UsuarioId);

        // Assert
        Assert.True(result);
        _repositorioMock.Verify(x => x.Delete(It.Is<ImagemPerfilUsuario>(i => i.Id == imagemPerfil.Id)), Times.Once);
        _mockAmazonS3Bucket.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<ImagemPerfilUsuario>()), Times.Once);
        _repositorioMock.Verify(x => x.Find(It.IsAny<Expression<Func<ImagemPerfilUsuario, bool>>>()), Times.Once);
    }


    [Fact]
    public async Task Delete_Should_Return_False_When_BucketDeleteFails()
    {
        // Arrange
        var imagemPerfil = _imagensPerfil.First();
        _repositorioMock.Setup(x => x.GetAll()).Returns(_imagensPerfil);
        _mockAmazonS3Bucket.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<ImagemPerfilUsuario>())).ReturnsAsync(false);

        // Act
        var result = await _imagemPerfilUsuarioBusiness.Delete(imagemPerfil.UsuarioId);

        // Assert
        Assert.False(result);
    }
}
