using CrossCutting.CommonDependenceInject;
using Application.CommonDependenceInject;
using WebApi.CommonDependenceInject;
using GlobalException.CommonDependenceInject;
using Infrastructure.CommonDependenceInject;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Repository.CommonDependenceInject;
using Migrations.Oracle.CommonInjectDependence;
using Migrations.MsSqlServer.CommonInjectDependence;
using Migrations.MySqlServer.CommonInjectDependence;

var builder = WebApplication.CreateBuilder(args);
// -------------------- Configuração de CORS --------------------
// Define as origens permitidas para requisições cross-origin
builder.AddCORSConfigurations();

// -------------------- Configuração de Routing e Controllers --------------------
builder.Services.AddRouting(options => options.LowercaseUrls = true); // URLs em minúsculo
builder.Services.AddControllers(); // Registra controllers
builder.Services.AddSwaggerApiVersioning(); // Swagger + versionamento de API

// -------------------- Configuração do DbContext Oralce --------------------
//builder.Services.ConfigureOracleServerMigrationsContext(builder.Configuration);

// -------------------- Configuração do DbContext Sql Server  --------------------
//builder.Services.ConfigureMsSqlServerMigrationsContext(builder.Configuration);

// -------------------- Configuração do DbContext MySql Server  --------------------
builder.Services.ConfigureMySqlServerMigrationsContext(builder.Configuration);


// -------------------- Configurações de Segurança --------------------
builder.AddSigningConfigurations(); // Configura assinaturas JWT
builder.AddAuthenticationConfigurations(); // Configura autenticação

// -------------------- Configurações de Criptografia --------------------
builder.Services.AddServicesCryptography(builder.Configuration);

// -------------------- Injeção de Dependências --------------------
builder.Services.AddAutoMapper(); // AutoMapper
builder.Services.AddAmazonS3BucketConfigurations(builder.Configuration); // Configuração S3
builder.Services.AddRepositories(); // Repositórios
builder.Services.AddServices(); // Serviços da aplicação
builder.Services.AddCrossCuttingConfiguration(); // Cross-cutting concerns

// -------------------- Logging --------------------
// Remove todos os providers (Console, Debug, EventLog) em Staging ou Production
if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
    builder.Logging.ClearProviders();

// -------------------- Health Checks --------------------
builder.AddHealthCheckConfigurations();


var app = builder.Build();

// -------------------- Middleware --------------------
app.UseGlobalExceptionHandler(); // Tratamento global de exceções
app.UseHsts(); // HTTPS Strict Transport Security
app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.AddSupporteCulturesPtBr(); // Suporte a culturas PT-BR
app.UseCors(); // Ativa CORS

if (!app.Environment.IsProduction()) 
    app.AddSwaggerUIApiVersioning(); // Swagger UI apenas para ambientes que não sejam produção 

app.UseDefaultFiles(); // Suporte a arquivos default (index.html)
app.UseStaticFiles(); // Servir arquivos estáticos
app.UseRouting(); // Habilita roteamento
app.UseCertificateForwarding(); // Forward de certificados
app.UseAuthentication(); // Autenticação
app.UseAuthorization(); // Autorização

// -------------------- Endpoints --------------------
app.MapHealthChecks("/health"); // Endpoint de health check
app.MapControllers(); // Mapeia controllers
app.MapFallbackToFile("index.html"); // Fallback para SPA


// -------------------- Configuração de URLs para Staging & Development -------------------- i
if (app.Environment.IsStaging() || app.Environment.IsDevelopment())
{
    app.Urls.Add("https://0.0.0.0:42535"); app.Urls.Add("http://0.0.0.0:42536");
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // necessário para a UI
    });

    app.UseHealthChecksUI(options =>
    {
        options.UIPath = "/health-ui"; // URL para acessar a interface
    });
}

// -------------------- Executa aplicação --------------------
app.Run();
