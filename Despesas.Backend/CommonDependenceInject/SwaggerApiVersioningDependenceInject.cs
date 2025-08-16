using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Despesas.Backend.CommonDependenceInject;

public static class SwaggerApiVersioningDependenceInject
{
    private readonly static string appName = "API Balanço Pessoal";
    private readonly static string currentVersion = "v0.0.1";
    private readonly static string appDescription = @$"
           A API Balanço Pessoal fornece serviços para o gerenciamento de finanças pessoais de forma simples e objetiva.  Por meio de seus endpoints, é possível registrar 
       gastos e receitas, consultar relatórios, obter gráficos consolidados e organizar informações financeiras de maneira prática e eficiente.
           Essa API foi projetada para permitir que aplicações clientes — como aplicativos móveis ou sistemas web — acessem e manipulem dados financeiros de forma segura 
       e estruturada, auxiliando usuários a tomarem decisões mais conscientes e alcançarem a estabilidade financeira com confiança.";

    public static void AddSwaggerApiVersioning(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Scheme = "Bearer",
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            c.SchemaFilter<HideDtosFilter>();


            c.SwaggerDoc(currentVersion, new OpenApiInfo
            {
                Title = appName,
                Version = currentVersion,
                Description = appDescription,
                Contact = new OpenApiContact
                {
                    Name = "Projeto Balanço Pessoal  - HONEY TI",
                    Url = new Uri("https://github.com/alexribeirofaria/prj-despesas-pessoais")
                },
            });

            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                var controllerNamespace = methodInfo?.DeclaringType?.Namespace;

                if (docName == currentVersion)
                    return true;

                return false;
            });
        });
    }

    public static void AddSwaggerUIApiVersioning(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
            c.SwaggerEndpoint(@$"{swaggerJsonBasePath}/swagger/{currentVersion}/swagger.json", $"{appName} ");
        });
    }
}

internal class HideDtosFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(BaseAuthenticationDto))
        {
            schema.Description = "DTO contendo informações de autenticação.";
            schema.Type = "object";
            schema.Properties.Clear();
        }

        if (context.Type == typeof(GoogleAuthenticationDto))
        {
            schema.Description = "DTO contendo informações de autenticação google.";
            schema.Type = "object";
            schema.Properties.Clear();
        }
        

        if (context.Type == typeof(ProblemDetails))
        {
            schema.Description = "Informações de erro retornado pela API.";
            schema.Type = "object";
            schema.Properties.Clear();
        }
    }
}