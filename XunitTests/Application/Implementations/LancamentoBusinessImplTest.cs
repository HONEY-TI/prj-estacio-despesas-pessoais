using AutoMapper;
using Repository.Persistency.Abstractions;
using __mock__.Entities;
using Despesas.Application.Implementations;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;

namespace Application;
public sealed class LancamentoBusinessImplTest
{
    private readonly Mock<ILancamentoRepositorio> _repositorioMock;
    private readonly LancamentoBusinessImpl<LancamentoDto> _lancamentoBusiness;
    private Mapper _mapper;

    public LancamentoBusinessImplTest()
    {
        _repositorioMock = new Mock<ILancamentoRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<LancamentoProfile>(); }));
        _lancamentoBusiness = new LancamentoBusinessImpl<LancamentoDto>(_mapper, _repositorioMock.Object);
    }

    [Fact]
    public async Task FindByMesAno_Should_Return_List_Of_LancamentoDto()
    {
        // Arrange            
        var lancamentos = LancamentoFaker.Lancamentos();
        var data = lancamentos.First().Data;
        var idUsuario = lancamentos.First().UsuarioId;
        _repositorioMock.Setup(r => r.FindByMesAno(data, idUsuario)).ReturnsAsync(lancamentos.FindAll(l => l.UsuarioId == idUsuario));

        // Act
        var result = await _lancamentoBusiness.FindByMesAno(data, idUsuario);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<LancamentoDto>>(result);
        Assert.Equal(lancamentos.FindAll(l => l.UsuarioId == idUsuario).Count, result.Count);
        _repositorioMock.Verify(r => r.FindByMesAno(data, idUsuario), Times.Once);
    }
}