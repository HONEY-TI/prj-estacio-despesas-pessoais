using AutoMapper;
using __mock__.Entities;
using Repository.Persistency.Generic;
using Despesas.Application.Dtos;
using Despesas.Application.Abstractions.Generic;
using Despesas.Application.Dtos.Profile;

namespace Application.Generic;
public sealed class GenericBusinessTests
{
    private Mock<IRepositorio<Categoria>> _mockRepositorio;
    private GenericBusiness<CategoriaDto, Categoria> _genericBusiness;
    private List<Categoria> _categorias;
    private Mapper _mapper;
    public GenericBusinessTests()
    {
        _mockRepositorio = new Mock<IRepositorio<Categoria>>(MockBehavior.Default);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<CategoriaProfile>(); }));
        _genericBusiness = new GenericBusiness<CategoriaDto, Categoria>(_mapper, _mockRepositorio.Object);
        _categorias = CategoriaFaker.Instance.Categorias();
    }

    [Fact]
    public void Create_Should_Return_Inserted_Object()
    {
        // Arrange
        var obj = _categorias.Last();
        var categoria = CategoriaFaker.Instance.CategoriasVMs().First();
        _mockRepositorio.Setup(repo => repo.Insert(It.IsAny<Categoria>()));

        // Act
        var result = _genericBusiness.Create(categoria);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CategoriaDto>(result);
        Assert.Equal(categoria.Id, result.Id);
        _mockRepositorio.Verify(repo => repo.Insert(It.IsAny<Categoria>()), Times.Once);
    }

    [Fact]
    public void FindAll_Should_Return_All_Objects()
    {
        // Arrange
        var objects = UsuarioFaker.Instance.GetNewFakersUsuarios();
        var repositoryMock = new Mock<IRepositorio<Usuario>>();
        repositoryMock.Setup(repo => repo.GetAll()).Returns(objects);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<UsuarioProfile>(); }));
        var business = new GenericBusiness<UsuarioDto, Usuario>(_mapper, repositoryMock.Object);

        // Act
        var result = business.FindAll(objects.First().Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(objects.Count, result.Count);
        Assert.IsType<List<UsuarioDto>>(result);
        repositoryMock.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public void FindById_Should_Return_Object_With_MatchingId()
    {
        // Arrange

        var obj = DespesaFaker.Instance.Despesas().First();
        var id = obj.Id;
        var repositoryMock = new Mock<IRepositorio<Despesa>>();
        repositoryMock.Setup(repo => repo.Get(id)).Returns(obj);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<DespesaProfile>(); }));
        var business = new GenericBusiness<DespesaDto, Despesa>(_mapper, repositoryMock.Object);

        // Act
        var result = business.FindById(id, obj.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(obj.Id, result.Id);
        Assert.IsType<DespesaDto>(result);
        repositoryMock.Verify(repo => repo.Get(id), Times.Once);
    }

    [Fact]
    public void Update_Should_Return_Updated_Object()
    {
        // Arrange
        var obj = ReceitaFaker.Instance.ReceitasVMs().First();
        var repositoryMock = new Mock<IRepositorio<Receita>>();
        repositoryMock.Setup(repo => repo.Update(It.IsAny<Receita>()));
        var mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<ReceitaProfile>(); }));
        var business = new GenericBusiness<ReceitaDto, Receita>(mapper, repositoryMock.Object);

        // Act
        var result = business.Update(obj);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(obj, result);
        Assert.IsType<ReceitaDto>(result);
        repositoryMock.Verify(repo => repo.Update(It.IsAny<Receita>()), Times.Once);
    }

    [Fact]
    public void Delete_Should_Return_True_If_Deleted_Successfully()
    {
        // Arrange
        var objects = UsuarioFaker.Instance.GetNewFakersUsuarios();
        var obj = objects.First();
        var repositoryMock = Usings.MockRepositorio(objects);
        repositoryMock.Setup(repo => repo.Delete(It.IsAny<Usuario>()));
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<UsuarioProfile>(); }));
        var business = new GenericBusiness<UsuarioDto, Usuario>(_mapper, repositoryMock.Object);

        // Act
        var result = business.Delete(_mapper.Map<UsuarioDto>(obj));

        // Assert
        Assert.IsType<bool>(result);
        Assert.True(result);
        repositoryMock.Verify(repo => repo.Delete(It.IsAny<Usuario>()), Times.Once);
    }
}