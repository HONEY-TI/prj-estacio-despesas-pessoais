using Despesas.Application.Abstractions;
using Despesas.Application.Abstractions.Generic;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Core;
using Despesas.Application.Implementations;
using Despesas.Infrastructure.Email;
using Despesas.Infrastructure.Email.Abstractions;
using Domain.Entities;
using EasyCryptoSalt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Persistency.UnitOfWork;
using Repository.Persistency.UnitOfWork.Abstractions;

namespace Despesas.Application.CommonDependenceInject;

public static class ServicesDependenceInject
{
    public static IServiceCollection AddServicesCryptography(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CryptoOptions>(configuration.GetSection("CryptoConfigurations"));
        services.AddSingleton<ICrypto, Crypto>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddScoped(typeof(IBusiness<CategoriaDto, Categoria>), typeof(CategoriaBusinessImpl<CategoriaDto>));
        services.AddScoped(typeof(IBusiness<DespesaDto, Despesa>), typeof(DespesaBusinessImpl<DespesaDto>));
        services.AddScoped(typeof(IBusiness<ReceitaDto, Receita>), typeof(ReceitaBusinessImpl<ReceitaDto>));
        services.AddScoped(typeof(IAcessoBusiness<AcessoDto, LoginDto>), typeof(AcessoBusinessImpl<AcessoDto, LoginDto>));
        services.AddScoped(typeof(ILancamentoBusiness<LancamentoDto>), typeof(LancamentoBusinessImpl<LancamentoDto>));
        services.AddScoped(typeof(IUsuarioBusiness<UsuarioDto>), typeof(UsuarioBusinessImpl<UsuarioDto>));
        services.AddScoped(typeof(IImagemPerfilUsuarioBusiness<ImagemPerfilDto, UsuarioDto>), typeof(ImagemPerfilUsuarioBusinessImpl<ImagemPerfilDto, UsuarioDto>));

        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped(typeof(IBusinessBase<CategoriaDto, Categoria>), typeof(CategoriaBusinessImpl<CategoriaDto>));
        services.AddScoped(typeof(IBusinessBase<DespesaDto, Despesa>), typeof(DespesaBusinessImpl<DespesaDto>));
        services.AddScoped(typeof(IBusinessBase<ReceitaDto, Receita>), typeof(ReceitaBusinessImpl<ReceitaDto>));

        services.AddScoped(typeof(IAcessoBusiness<AcessoDto, LoginDto>), typeof(AcessoBusinessImpl<AcessoDto, LoginDto>));
        services.AddScoped(typeof(ILancamentoBusiness<LancamentoDto>), typeof(LancamentoBusinessImpl<LancamentoDto>));
        services.AddScoped(typeof(IUsuarioBusiness<UsuarioDto>), typeof(UsuarioBusinessImpl<UsuarioDto>));
        services.AddScoped(typeof(IImagemPerfilUsuarioBusiness<ImagemPerfilDto, UsuarioDto>), typeof(ImagemPerfilUsuarioBusinessImpl<ImagemPerfilDto, UsuarioDto>));
        services.AddScoped(typeof(ISaldoBusiness), typeof(SaldoBusinessImpl));
        services.AddScoped(typeof(IGraficosBusiness), typeof(GraficosBusinessImpl));
        services.AddScoped<IEmailSender, EmailSender>();
        return services;
    }
}


