using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;

namespace Application.Dtos;
public sealed class DespesaDtoTest
{
    [Fact]
    public void DespesaDto_Should_Set_Properties_Correctly()
    {
        // Arrange 
        var mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.AddProfile<DespesaProfile>();
        }));

        // Act
        var despesa = DespesaFaker.Instance.Despesas().First();
        var despesaDto = mapper.Map<DespesaDto>(despesa);

        // Assert
        Assert.Equal(despesa.Id, despesaDto.Id);
        Assert.Equal(despesa.Data, despesaDto.Data);
        Assert.Equal(despesa.Descricao, despesaDto.Descricao);
        Assert.Equal(despesa.Valor, despesaDto.Valor);
        Assert.Equal(despesa.DataVencimento, despesaDto.DataVencimento);
        Assert.Equal(despesa.UsuarioId, despesaDto.UsuarioId);
        Assert.Equal(despesa?.Categoria?.Id, despesaDto.CategoriaId);
    }

}