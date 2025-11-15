using Application.CommonDependenceInject;
using Application.Dtos.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace CommonDependenceInject;
public class AutoMapperInjectDependenceTest
{
    [Fact]
    public void AddAutoMapper_Should_Register_Profiles()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAutoMapper();

        // Assert
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(AcessoProfile)));
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(CategoriaProfile)));        
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(DespesaProfile)));
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(ImagemPerfilUsuarioProfile)));
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(LancamentoProfile)));
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(LancamentoProfile)));
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(ReceitaProfile)));
        Assert.NotNull(services?.Any(descriptor => descriptor.ServiceType == typeof(UsuarioProfile)));
    }
}
