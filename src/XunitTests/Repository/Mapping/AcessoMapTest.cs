using Repository.Mapping;
using Repository.Mapping.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Repository.Mapping;
public sealed class AcessoMapTest
{
    [Fact]
    public void EntityConfiguration_IsValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "AcessoMapTest").Options;

        using (var context = new RegisterContext(options, DatabaseProvider.MySql, Usings.GetLogerFactory()))
        {
            var builder = new ModelBuilder(new ConventionSet());
            var configuration = new AcessoMap(DatabaseProvider.MySql);

            // Act
            configuration.Configure(builder.Entity<Acesso>());

            var model = builder.Model;
            var entityType = model.FindEntityType(typeof(Acesso));

            var idProperty = entityType?.FindProperty("Id");

            var loginProperty = entityType?.FindProperty("Login");
            var usuarioIdProperty = entityType?.FindProperty("UsuarioId");
            var index = new[] { loginProperty };
            var loginIndex = entityType?.FindIndex(index);

            // Assert
            Assert.NotNull(idProperty);
            Assert.NotNull(loginProperty);
            Assert.NotNull(usuarioIdProperty);
            Assert.NotNull(loginIndex);
            Assert.True(idProperty.IsPrimaryKey());
            Assert.False(idProperty.IsNullable);
            Assert.False(loginProperty.IsNullable);
            Assert.Equal(100, loginProperty.GetMaxLength());
            Assert.True(loginIndex.IsUnique);
        }
    }
}
