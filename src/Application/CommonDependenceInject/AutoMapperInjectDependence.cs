using Application.Dtos.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace Application.CommonDependenceInject;

public static class AutoMapperInjectDependence
{
    public static IServiceCollection AddApplicationAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(
                    cfg => { },
                    typeof(AcessoProfile),
                    typeof(CategoriaProfile),
                    typeof(DespesaProfile),
                    typeof(ImagemPerfilUsuarioProfile),
                    typeof(LancamentoProfile),
                    typeof(ReceitaProfile),
                    typeof(UsuarioProfile)
                );

        return services;
    }
}
