using __mock__.Entities;
using AutoMapper;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Profile;

namespace Application.Dtos;
public sealed class CategoriaDtoTest
{

    [Fact]
    
    public void CategoriaDto_Should_Set_Properties_Correctly()
    {
        // Arrange        
        var mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<CategoriaProfile>(); }));        

        // Act
        var categoria = CategoriaFaker.Instance.Categorias().First();
        var categoriaDto = mapper.Map<CategoriaDto>(categoria);


        // Assert
        Assert.Equal(categoria.Id, categoriaDto.Id);
        Assert.Equal(categoria.Descricao, categoriaDto.Descricao);
        Assert.Equal(categoria.UsuarioId, categoriaDto.UsuarioId);
        Assert.Equal(categoria.TipoCategoria.Id, (int)categoriaDto.IdTipoCategoria);
    }
}