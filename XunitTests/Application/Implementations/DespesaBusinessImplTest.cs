using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Dtos;
using Despesas.Application.Implementations;
using Despesas.GlobalException.CustomExceptions;
using Despesas.Repository.UnitOfWork.Abstractions;
using Domain.Core.ValueObject;
using Repository.Persistency.Generic;
using System.Linq.Expressions;

namespace Application;

public sealed class DespesaBusinessImplTest
{
    private readonly Mock<IUnitOfWork<Despesa>> _unitOfWork;
    private readonly Mock<IUnitOfWork<Categoria>> _unitOfWorkCategoria;    
    private readonly Mock<IRepositorio<Despesa>> _repositorioMock;
    private readonly DespesaBusinessImpl<DespesaDto> _despesaBusiness;
    private Mapper _mapper;

    public DespesaBusinessImplTest()
    {
        _unitOfWork = new Mock<IUnitOfWork<Despesa>>(MockBehavior.Default);
        _unitOfWorkCategoria = new Mock<IUnitOfWork<Categoria>>(MockBehavior.Default);
        _repositorioMock = new Mock<IRepositorio<Despesa>>();
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<DespesaProfile>(); }));
        _despesaBusiness = new DespesaBusinessImpl<DespesaDto>(_mapper, _repositorioMock.Object, _unitOfWork.Object, _unitOfWorkCategoria.Object);
    }

    [Fact]
    public async Task Create_Should_Returns_Parsed_Despesa_VM()
    {
        // Arrange
        var despesa = DespesaFaker.Instance.Despesas().First();
        var despesaDto = _mapper.Map<DespesaDto>(despesa);        
        var categorias = CategoriaFaker.Instance.Categorias(despesa.Usuario, (int)TipoCategoria.CategoriaType.Despesa, despesa.UsuarioId);
        categorias.Add(despesa.Categoria ?? new());
        _unitOfWork.Setup(repo => repo.Repository.Insert(It.IsAny<Despesa>()));
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(despesa);
        _unitOfWorkCategoria.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(categorias.First(c => c.Id ==despesaDto.CategoriaId));

        // Act
        var result = await _despesaBusiness.Create(despesaDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DespesaDto>(result);
        Assert.Equal(despesaDto.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Insert(It.IsAny<Despesa>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWorkCategoria.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeastOnce);
        _unitOfWork.Verify(repo => repo.Repository.Insert(It.IsAny<Despesa>()), Times.Once);
    }

    [Fact]
    public async Task FindAll_Should_Returns_List_Of_DespesaDto()
    {
        // Arrange                     
        var despesas = DespesaFaker.Instance.Despesas();
        var despesa = despesas.First();
        var idUsuario = despesa.UsuarioId;
        despesas = despesas.FindAll(d => d.UsuarioId == idUsuario);
        _repositorioMock.Setup(repo => repo.GetAll()).Returns(despesas);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Despesa, bool>>>())).ReturnsAsync(despesas);

        // Act
        var result = await _despesaBusiness.FindAll(idUsuario);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<DespesaDto>>(result);
        Assert.Equal(despesas.Count, result.Count);
        _repositorioMock.Verify(repo => repo.GetAll(), Times.Never);
        _unitOfWork.Verify(uow => uow.Repository.Find(It.IsAny<Expression<Func<Despesa, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task FindById_Should_Returns_Parsed_DespesaDto()
    {
        // Arrange
        var _despesas = DespesaFaker.Instance.Despesas();
        var despesa = _despesas.First();
        var id = despesa.Id;
        _repositorioMock.Setup(repo => repo.Get(id)).Returns(despesa);
        _unitOfWork.Setup(u => u.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(despesa);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Despesa, bool>>>())).ReturnsAsync(_despesas);

        // Act
        var result = await _despesaBusiness.FindById(id, despesa.UsuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DespesaDto>(result);
        Assert.Equal(despesa.Id, result.Id);
        _repositorioMock.Verify(repo => repo.Get(id), Times.Never);
        _unitOfWork.Verify(u => u.Repository.Get(It.IsAny<Guid>()), Times.AtLeast(1));
    }

    [Fact]
    public async Task FindById_Should_Returns_Null_When_Parsed_DespesaDto()
    {
        // Arrange
        var _despesas = DespesaFaker.Instance.Despesas();
        var despesa = _despesas.First();
        var id = despesa.Id;
        _repositorioMock.Setup(repo => repo.Get(id)).Returns(() => null);
        _unitOfWork.Setup(u => u.Repository.Get(It.IsAny<Guid>())).ReturnsAsync((Despesa)null);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Despesa, bool>>>())).ReturnsAsync((List<Despesa>)null);

        // Act
        var result = await _despesaBusiness.FindById(id, Guid.Empty);

        // Assert
        Assert.Null(result);
        _repositorioMock.Verify(repo => repo.Get(id), Times.Never);
        _unitOfWork.Verify(uow => uow.Repository.Find(It.IsAny<Expression<Func<Despesa, bool>>>()), Times.Once);
        _unitOfWork.Verify(u => u.Repository.Get(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Update_Should_Returns_Parsed_DespesaDto()
    {
        // Arrange         
        var despesas = DespesaFaker.Instance.Despesas();
        var despesa = despesas.First();
        despesa.Descricao = "Teste Update Despesa";
        var despesaDto = _mapper.Map<DespesaDto>(despesa);
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(despesa);
        _unitOfWork.Setup(u => u.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(despesa);
        _unitOfWorkCategoria.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(despesa.Categoria);

        // Act
        var result = await _despesaBusiness.Update(despesaDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DespesaDto>(result);
        Assert.Equal(despesa.Id, result.Id);
        Assert.Equal(despesa.Descricao, result.Descricao);
        _repositorioMock.Verify(repo => repo.Update(It.IsAny<Despesa>()), Times.Never);
        _unitOfWork.Verify(u => u.Repository.Get(It.IsAny<Guid>()), Times.AtLeast(2));
        _unitOfWorkCategoria.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.Once);
        _unitOfWork.Verify(repo => repo.Repository.Update(It.IsAny<Despesa>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Returns_True()
    {
        // Arrange
        var _despesas = DespesaFaker.Instance.Despesas();
        var despesa = _despesas.First();
        _repositorioMock.Setup(repo => repo.Delete(It.IsAny<Despesa>()));
        _repositorioMock.Setup(repo => repo.Get(It.IsAny<Guid>())).Returns(despesa);
        _unitOfWork.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync(despesa);
        _unitOfWork.Setup(uow => uow.Repository.Find(It.IsAny<Expression<Func<Despesa, bool>>>())).ReturnsAsync(_despesas);

        // Act
        var despesaDto = _mapper.Map<DespesaDto>(despesa);
        var result = await _despesaBusiness.Delete(despesaDto);

        // Assert
        Assert.IsType<bool>(result);
        Assert.True(result);
        _repositorioMock.Verify(repo => repo.Delete(It.IsAny<Despesa>()), Times.Never);
        _unitOfWork.Verify(repo => repo.Repository.Get(It.IsAny<Guid>()), Times.AtLeastOnce);
        _unitOfWork.Verify(repo => repo.Repository.Delete(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task IsCategoriaValid_Should_Throws_Exception()
    {
        // Arrange
        var despesa = DespesaFaker.Instance.Despesas().First();
        var despesaDto = _mapper.Map<DespesaDto>(despesa);
        _unitOfWorkCategoria.Setup(repo => repo.Repository.Get(It.IsAny<Guid>())).ReturnsAsync((Categoria)null);

        // Act & Assert 
        await Assert.ThrowsAsync<CategoriaUsuarioInvalidaException>(() => _despesaBusiness.Create(despesaDto));
    }
}
