using Repository.Mapping.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public sealed class RegisterContextTest
{
    [Fact]
    public void RegisterContext_Should_Have_DbSets()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "RegisterContext_Should_Have_DbSets").Options;

        // Act
        using (var context = new RegisterContext(options, DatabaseProvider.MySql, Usings.GetLogerFactory()))
        {
            // Assert
            Assert.NotNull(context.Acesso);
            Assert.NotNull(context.Usuario);
            Assert.NotNull(context.ImagemPerfilUsuario);
            Assert.NotNull(context.Despesa);
            Assert.NotNull(context.Receita);
            Assert.NotNull(context.Categoria);
            Assert.NotNull(context.Lancamento);
        }
    }

    [Fact]
    public void RegisterContext_Should_Apply_Configurations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "RegisterContext_Should_Apply_Configurations").Options;

        // Act
        using (var context = new RegisterContext(options, DatabaseProvider.MySql, Usings.GetLogerFactory()))
        {
            // Assert
            var model = context.Model;
            Assert.True(model.FindEntityType(typeof(Categoria)) != null);
            Assert.True(model.FindEntityType(typeof(Usuario)) != null);
            Assert.True(model.FindEntityType(typeof(ImagemPerfilUsuario)) != null);
            Assert.True(model.FindEntityType(typeof(Acesso)) != null);
            Assert.True(model.FindEntityType(typeof(Despesa)) != null);
            Assert.True(model.FindEntityType(typeof(Receita)) != null);
            Assert.True(model.FindEntityType(typeof(Lancamento)) != null);
        }
    }
}
