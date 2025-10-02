using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;
using Despesas.Application.Implementations;
using Despesas.GlobalException.CustomExceptions;
using Despesas.Repository.UnitOfWork.Abstractions;
using Repository.Persistency.Generic;
using System.Linq.Expressions;

namespace Application;

public sealed class ReceitaBusinessImplTest
{
    private readonly Mock<IRepositorio<Receita>> _repositorioMock;
    private readonly ReceitaBusinessImpl<ReceitaDto> _receitaBusiness;
    private readonly Mock<IUnitOfWork<Receita>> _unitOfWork;
    private readonly Mock<IUnitOfWork<Categoria>> _unitOfWorkCategoria;
    private Mapper _mapper;

    public ReceitaBusinessImplTest()
    {
        _repositorioMock = new Mock<IRepositorio<Receita>>();
        _unitOfWork = new Mock<IUnitOfWork<Receita>>();
        _unitOfWorkCategoria = new Mock<IUnitOfWork<Categoria>>(MockBehavior.Default);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<ReceitaProfile>(); }));
        _receitaBusiness = new ReceitaBusinessImpl<ReceitaDto>(_mapper, _repositorioMock.Object, _unitOfWork.Object, _unitOfWorkCategoria.Object);
    }

    [Fact]
    public async Task Create_Should_Returns_Parsed_ReceitaDto()
    {
        // Arrange
        var receitas = ReceitaFaker.Instance.Receitas();
        var receita = receitas.First();
        var receitaDto = _mapper.Map<ReceitaDto>(receita);
        _repositorioMock.Setup(repo => repo.Insert(It.IsAny<Receita>()));
        _unitOfWork.Setup(repo => repo.Repository.Insert(It.IsAny<Receita>()));
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(receita);
        _unitOfWorkCategoria.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(receita.Categoria);

        // Act
        var result = await _receitaBusiness.Create(receitaDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ReceitaDto>(result);
        Assert.Equal(receitaDto.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Insert(It.IsAny<Receita>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWorkCategoria.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);        
        _unitOfWork.Verify(repo => repo.Repository.Insert(It.IsAny<Receita>()), Times.Once);

    }

    [Fact]
    public async Task FindAll_Should_Returns_List_Of_ReceitaDto()
    {
        // Arrange         
        var receitas = ReceitaFaker.Instance.Receitas();
        var receita = receitas.Last();
        var idUsuario = receita.UsuarioId;
        receitas = receitas.FindAll(r => r.UsuarioId == idUsuario);
        _repositorioMock.Setup(repo => repo.GetAll()).Returns(receitas);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Receita, bool>>>())).ReturnsAsync(receitas);

        // Act
        var result = await _receitaBusiness.FindAll(idUsuario);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<ReceitaDto>>(result);
        Assert.Equal(receitas.Count, result.Count);
        _repositorioMock.Verify(repo => repo.GetAll(), Times.Never);
        _unitOfWork.Verify(uow => uow.Repository.Find(It.IsAny<Expression<Func<Receita, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task FindById_Should_Returns_Parsed_ReceitaDto()
    {
        // Arrange        
        var receita = ReceitaFaker.Instance.Receitas().First();
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(receita);
        _unitOfWork.Setup(u => u.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(receita);

        // Act
        var result = await _receitaBusiness.FindById(receita.Id, receita.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ReceitaDto>(result);
        Assert.Equal(receita.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Get(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.Repository.Get(It.IsAny<Guid>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task FindById_Should_Returns_Null_When_Parsed_ReceitaDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var receita = ReceitaFaker.Instance.Receitas()[0];
        _repositorioMock.Setup(repo => repo.Get(id)).Returns(() => null);
        _unitOfWork.Setup(u => u.Repository.Get(It.IsAny<Guid>())).ReturnsAsync((Receita)null);

        // Act
        var result = await _receitaBusiness.FindById(id, Guid.Empty);

        // Assert
        Assert.Null(result);
        _repositorioMock.Verify(repo => repo.Get(id), Times.Never);
        _unitOfWork.Verify(u => u.Repository.Get(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Update_Should_Returns_Parsed_ReceitaDto()
    {
        // Arrange
        var receitas = ReceitaFaker.Instance.Receitas();
        var receita = receitas.First();
        var receitaDto = _mapper.Map<ReceitaDto>(receita);
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(receita);
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(receita);
        _unitOfWorkCategoria.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(receita.Categoria);


        // Act
        var result = await _receitaBusiness.Update(receitaDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ReceitaDto>(result);
        Assert.Equal(receita.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Update(It.IsAny<Receita>()), Times.Never);
        _unitOfWorkCategoria.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeast(2));
        _unitOfWork.Verify(repo => repo.Repository.Update(It.IsAny<Receita>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Returns_True()
    {
        // Arrange
        var receitas = ReceitaFaker.Instance.Receitas();
        var receita = receitas.First();
        var receitaDto = _mapper.Map<ReceitaDto>(receita);
        _repositorioMock.Setup(repo => repo.Delete(It.IsAny<Receita>()));
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(receita);
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(receita);

        // Act
        var result = await _receitaBusiness.Delete(receitaDto);

        // Assert
        Assert.IsType<bool>(result);
        Assert.True(result);
        _repositorioMock.Verify(repo => repo.Delete(It.IsAny<Receita>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeastOnce);
        _unitOfWork.Verify(repo => repo.Repository.Delete(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task IsCategoriaValid_Should_Throws_Exeption()
    {
        // Arrange
        var receita = ReceitaFaker.Instance.Receitas().First();
        var receitaDto = _mapper.Map<ReceitaDto>(receita);
        _unitOfWorkCategoria.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync((Categoria)null);

        // Act & Assert 
        await Assert.ThrowsAsync<CategoriaUsuarioInvalidaException>(async () => await _receitaBusiness.Create(receitaDto));    
    }
}