using Despesas.Application.Dtos.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace Despesas.Application.CommonDependenceInject;

public static class AutoMapperInjectDependence
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AcessoProfile).Assembly);
        services.AddAutoMapper(typeof(CategoriaProfile).Assembly);
        services.AddAutoMapper(typeof(DespesaProfile).Assembly);
        services.AddAutoMapper(typeof(ImagemPerfilUsuarioProfile).Assembly);
        services.AddAutoMapper(typeof(LancamentoProfile).Assembly);
        services.AddAutoMapper(typeof(ReceitaProfile).Assembly);
        services.AddAutoMapper(typeof(UsuarioProfile).Assembly);
        return services;
    }
}
