using CrossCutting.CQRS.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CrossCutting.CommonDependenceInject;
public static class CrossCuttingDependenceInject
{
    public static IServiceCollection AddCrossCuttingConfiguration(this IServiceCollection services)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssembly));
        services.AddTransient<IRequestHandler<GetAllQuery<Categoria>, IEnumerable<Categoria>>, GetAllQueryHandler<Categoria>>();

        return services;
    }
}