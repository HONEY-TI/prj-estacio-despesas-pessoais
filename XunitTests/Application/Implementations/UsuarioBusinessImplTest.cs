using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using Despesas.Application.Implementations;
using Despesas.GlobalException.CustomExceptions.Core;
using Despesas.Repository.UnitOfWork.Abstractions;
using Domain.Core.ValueObject;
using Domain.Entities;
using Moq;
using Repository.Persistency.Generic;
using System.Linq.Expressions;

namespace Application;

public sealed class UsuarioBusinessImplTest
{
    private readonly Mock<IRepositorio<Usuario>> _repositorioMock;
    private readonly Mock<IUnitOfWork<Usuario>> _unitOfWork;
    private readonly UsuarioBusinessImpl<UsuarioDto> _usuarioBusiness;
    private List<Usuario> _usuarios;
    private Mapper _mapper;
    public UsuarioBusinessImplTest()
    {
        _repositorioMock = new Mock<IRepositorio<Usuario>>();
        _unitOfWork = new Mock<IUnitOfWork<Usuario>>();
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<UsuarioProfile>(); }));
        _usuarioBusiness = new UsuarioBusinessImpl<UsuarioDto>(_mapper, _repositorioMock.Object, _unitOfWork.Object);
        _usuarios = UsuarioFaker.Instance.GetNewFakersUsuarios();
    }

    [Fact]
    public async Task Create_Should_Returns_Parsed_UsuarioDto()
    {
        // Arrange
        var usuario = _usuarios.First();
        usuario.PerfilUsuario = new PerfilUsuario(PerfilUsuario.Perfil.Admin);
        _repositorioMock.Setup(repo => repo.Insert(It.IsAny<Usuario>()));
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(usuario);
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(usuario);
        var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
        usuarioDto.UsuarioId = usuario.Id;

        // Act
        var result = await _usuarioBusiness.Create(usuarioDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UsuarioDto>(result);
        //Assert.Equal(usuario.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Insert(It.IsAny<Usuario>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeast(2));
        _unitOfWork.Verify(uow => uow.Repository.Insert(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task FindAll_Should_Returns_List_of_UsuarioDto()
    {
        // Arrange         
        var usuarios = _usuarios.Where(u => u.PerfilUsuario == PerfilUsuario.Perfil.Admin).ToList();
        var usuario = usuarios.First();
        var idUsuario = usuario.Id;
        _repositorioMock.Setup(repo => repo.GetAll()).Returns(_usuarios);
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuarios.AsEnumerable());
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(usuario);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Usuario, bool>>>())).ReturnsAsync(usuarios);
        _unitOfWork.Setup(uow => uow.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(usuario);
        _unitOfWork.Setup(uow => uow.Repository.GetAll()).ReturnsAsync(_usuarios);

        // Act
        var result = await _usuarioBusiness.FindAll(idUsuario);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<UsuarioDto>>(result);
        Assert.Equal(_usuarios.Count, result.Count);
        _repositorioMock.Verify(repo => repo.GetAll(), Times.Never);
        _repositorioMock.Verify(repo => repo.Find(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Never);
        _unitOfWork.Verify(uow => uow.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWork.Verify(uow => uow.Repository.Find(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
        _unitOfWork.Verify(uow => uow.Repository.GetAll(), Times.Once);
    }

    [Fact(Skip = "Disabled após implemntação DRY")]
    public void FindAll_Should_Returns_Thwors_Exception()
    {
        // Arrange         
        var usuarios = _usuarios.Where(u => u.PerfilUsuario == PerfilUsuario.Perfil.User);
        var usuario = usuarios.First();
        var idUsuario = usuario.Id;
        _repositorioMock.Setup(repo => repo.GetAll()).Returns(_usuarios);
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuarios.AsEnumerable());
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(usuario);

        // Act & Assert 
        Assert.Throws<ArgumentException>(() => _usuarioBusiness.FindAll(idUsuario).Result);
        _repositorioMock.Verify(repo => repo.GetAll(), Times.Never);
        _repositorioMock.Verify(repo => repo.Find(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task FindById_Should_Returns_Parsed_UsuarioDto()
    {
        // Arrange
        var usuarios = _usuarios.Take(3);
        var usuario = usuarios.First();
        var idUsuario = usuario.Id;
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(_usuarios.AsEnumerable());
        _repositorioMock.Setup(repo => repo.Get(idUsuario)).Returns(usuario);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Usuario, bool>>>())).ReturnsAsync(usuarios);

        // Act
        var result = await _usuarioBusiness.FindById(idUsuario);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UsuarioDto>(result);
        Assert.Equal(usuario.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Find(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Never);
        _unitOfWork.Verify(uow => uow.Repository.Find(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Update_Should_Returns_Parsed_UsuarioDto()
    {
        // Arrange            
        var usuario = _usuarios.First();
        var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
        usuario.Nome = "Teste Usuario Update";
        _repositorioMock.Setup(repo => repo.Update(It.IsAny<Usuario>()));
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(usuario);
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(usuario);
        // Act
        var result = await _usuarioBusiness.Update(usuarioDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UsuarioDto>(result);
        Assert.Equal(usuarioDto.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Update(It.IsAny<Usuario>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWork.Verify(repo => repo.Repository.Update(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Returns_True_when_Usuario_is_Administrador()
    {
        // Arrange
        var usuario = _usuarios.First(u => u.PerfilUsuario == PerfilUsuario.Perfil.Admin);
        var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
        usuarioDto.PerfilUsuario = 2;
        _repositorioMock.Setup(repo => repo.Delete(It.IsAny<Usuario>()));
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(usuario);
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(usuario);

        // Act
        var result = await _usuarioBusiness.Delete(usuarioDto);

        // Assert
        Assert.IsType<bool>(result);
        Assert.True(result);
        _repositorioMock.Verify(repo => repo.Delete(It.IsAny<Usuario>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeastOnce);
        _unitOfWork.Verify(repo => repo.Repository.Delete(It.IsAny<Guid>()), Times.Once);
    }


    [Fact]
    public async Task Delete_Should_Throw_Error_When_Usuario_Is_Not_Administrador()
    {
        // Arrange
        var usuario = _usuarios.First(u => u.PerfilUsuario.Id == (int)PerfilUsuario.Perfil.User);
        var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
        _unitOfWork.Setup(uow => uow.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(usuario);
        _repositorioMock.Setup(repo => repo.Delete(It.IsAny<Usuario>())).Verifiable();

        // Act & Assert 
        var exception = await Assert.ThrowsAsync<UsuarioNaoAutorizadoException>(() => _usuarioBusiness.Delete(usuarioDto));
        Assert.Equal("Usuário não permitido a realizar operação!", exception.Message);
        _repositorioMock.Verify(repo => repo.Delete(It.IsAny<Usuario>()), Times.Never);
        _unitOfWork.Verify(uow => uow.Repository.Get(It.IsAny<Guid>()), Times.Once);
    }

}