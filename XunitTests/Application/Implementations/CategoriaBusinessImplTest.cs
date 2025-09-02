using AutoMapper;
using MediatR;
using __mock__.Entities;
using System.Linq.Expressions;
using Repository.Persistency.Generic;
using Despesas.Application.Implementations;
using Despesas.Application.Dtos;
using Despesas.Repository.UnitOfWork.Abstractions;

namespace Application;

public sealed class CategoriaBusinessImplTest
{
    private readonly Mock<IUnitOfWork<Categoria>> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IRepositorio<Categoria>> _repositorioMock;
    private readonly CategoriaBusinessImpl<CategoriaDto> _categoriaBusiness;
    private readonly List<Categoria> _categorias;
    private Mapper _mapper;

    public CategoriaBusinessImplTest()
    {
        var usuario = UsuarioFaker.Instance.GetNewFaker();
        _categorias = CategoriaFaker.Instance.Categorias(usuario);
        _repositorioMock = Usings.MockRepositorio(_categorias);
        _unitOfWorkMock = new Mock<IUnitOfWork<Categoria>>(MockBehavior.Default);
        _mediator = new Mock<IMediator>(MockBehavior.Default);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<CategoriaProfile>(); }));
        _categoriaBusiness = new CategoriaBusinessImpl<CategoriaDto>(_mediator.Object, _mapper, _unitOfWorkMock.Object, _repositorioMock.Object);
    }

    [Fact]
    public async Task Create_Shloud_Returns_Parsed_CategoriaDto()
    {
        // Arrange
        var categoria = _categorias.First();
        var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
        _repositorioMock.Setup(repo => repo.Insert(It.IsAny<Categoria>()));
        _unitOfWorkMock.Setup(repo => repo.Repository.Insert(It.IsAny<Categoria>()));
        _unitOfWorkMock.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(categoria);

        // Act
        var result = await _categoriaBusiness.Create(categoriaDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CategoriaDto>(result);
        Assert.Equal(categoriaDto.Id, result.Id);
        _unitOfWorkMock.Verify(repo => repo.Repository.Insert(It.IsAny<Categoria>()), Times.Once);
        _unitOfWorkMock.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task FindAll_Should_Returns_List_Of_CategoriaDto()
    {
        // Arrange
        var categoria = _categorias.First();
        var mockCategorias = _categorias.FindAll(obj => obj.UsuarioId == categoria.UsuarioId);
        _repositorioMock.Setup(repo => repo.GetAll()).Returns(mockCategorias);
        _unitOfWorkMock.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Categoria, bool>>>()))
            .ReturnsAsync(mockCategorias);

        // Act
        var result = await _categoriaBusiness.FindAll(categoria.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<CategoriaDto>>(result);
        Assert.Equal(mockCategorias.Count, result.Count);
        _unitOfWorkMock.Verify(repo => repo.Repository.Find(It.IsAny<Expression<Func<Categoria, bool>>>()), Times.Once);
        _repositorioMock.Verify(repo => repo.GetAll(), Times.Never);
    }

    [Fact]
    public async Task FindById_Should_Returns_Parsed_CategoriaDto()
    {
        // Arrange
        var categoria = _categorias.First();
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Categoria, bool>>>())).Returns(_categorias.AsEnumerable());
        _unitOfWorkMock.Setup(repo => repo.Repository.Find(It.IsAny<Expression<Func<Categoria, bool>>>())).ReturnsAsync(_categorias.AsEnumerable());


        // Act
        var result = await _categoriaBusiness.FindById(categoria.Id, categoria.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CategoriaDto>(result);
        Assert.Equal(categoria.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Find(It.IsAny<Expression<Func<Categoria, bool>>>()), Times.Never);
        _unitOfWorkMock.Verify(repo => repo.Repository.Find(It.IsAny<Expression<Func<Categoria, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task FindById_Should_Returns_Empty_Categoria()
    {
        // Arrange

        var categoria = _categorias.First();
        _repositorioMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Categoria, bool>>>())).Returns(Enumerable.Empty<Categoria>());
        _unitOfWorkMock.Setup(repo => repo.Repository.Find(It.IsAny<Expression<Func<Categoria, bool>>>())).ReturnsAsync(Enumerable.Empty<Categoria>());

        // Act
        var result = await _categoriaBusiness.FindById(Guid.Empty, categoria.UsuarioId);

        // Assert
        Assert.Null(result);
        _repositorioMock.Verify(repo => repo.Find(It.IsAny<Expression<Func<Categoria, bool>>>()), Times.Never);
        _unitOfWorkMock.Verify(repo => repo.Repository.Find(It.IsAny<Expression<Func<Categoria, bool>>>()), Times.Once);
    }


    [Fact]
    public async Task Update_Should_Returns_Parsed_CategoriaDto()
    {
        // Arrange
        var categoria = CategoriaFaker.Instance.GetNewFaker(UsuarioFaker.Instance.GetNewFaker());
        var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(categoria);
        _repositorioMock.Setup(repo => repo.Update(It.IsAny<Categoria>()));
        _unitOfWorkMock.Setup(repo => repo.Repository.Update(It.IsAny<Categoria>()));
        _unitOfWorkMock.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(categoria);

        // Act
        var result = await _categoriaBusiness.Update(categoriaDto) as CategoriaDto;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CategoriaDto>(result);
        Assert.Equal(categoria.Id, result.Id);
        _unitOfWorkMock.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeastOnce);
        _unitOfWorkMock.Verify(repo => repo.Repository.Update(It.IsAny<Categoria>()), Times.Once);
        _repositorioMock.Verify(repo => repo.Update(It.IsAny<Categoria>()), Times.Never);
        _repositorioMock.Verify(repo => repo.Get(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Returns_True()
    {
        // Arrange
        var categoria = _categorias.First();
        var objToDelete = _mapper.Map<CategoriaDto>(categoria);
        _repositorioMock.Setup(repo => repo.Delete(It.IsAny<Categoria>()));
        _unitOfWorkMock.Setup(repo => repo.Repository.Delete(It.IsAny<Guid>()));
        _unitOfWorkMock.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(categoria);

        // Act
        var result = await _categoriaBusiness.Delete(objToDelete);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWorkMock.Verify(repo => repo.Repository.Delete(It.IsAny<Guid>()), Times.Once);        
    }
}
