using Despesas.Application.Authentication;
using Despesas.Business.Authentication.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Despesas.Backend.CommonDependenceInject;

public static class AutorizationDependenceInject
{
    private static readonly string[] PRODUCTION_ORIGINGS =  { 
        "https://alexfariakof.com" 
    };

    private static readonly string[] DEVELOPMENT_ORIGINGS = {
        "https://alexfariakof.com",
        "https://alexfariakof.com:42535",
        "https://localhost",
        "https://localhost:42535",
        "https://localhost:4200",
        "https://127.0.0.1",
        "https://127.0.0.1:4200",
        "https://127.0.0.1:42535",
        "https://accounts.google.com"
    };

    public static void AddCORSConfigurations(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Environment.IsProduction()
            ? PRODUCTION_ORIGINGS
            : DEVELOPMENT_ORIGINGS;

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });
    }

    public static void AddSigningConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenConfigurations"));
        var options = builder.Services.BuildServiceProvider().GetService<IOptions<TokenOptions>>();
        var certificate = LoadCertificate(options);

        var signingConfigurations = new SigningConfigurations(certificate, options);
        builder.Services.AddSingleton<SigningConfigurations>(signingConfigurations);

        if (builder.Environment.IsProduction())
        {
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                {
                    httpsOptions.ServerCertificate = certificate;
                });
            });
        }
    }

    public static void AddAuthenticationConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(bearerOptions =>
        {
            var options = builder.Services.BuildServiceProvider().GetService<IOptions<TokenOptions>>();

            bearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = builder.Services.BuildServiceProvider().GetService<SigningConfigurations>().Key,
                ValidAudience = options.Value.Audience,
                ValidIssuer = options.Value.Issuer,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​).RequireAuthenticatedUser().Build());
        });
    }

    private static X509Certificate2? LoadCertificate(IOptions<TokenOptions>? options)
    {
        if (options?.Value == null || string.IsNullOrWhiteSpace(options.Value.Certificate))
            return null;

        string certificatePath = Path.Combine(AppContext.BaseDirectory, options.Value.Certificate);

        if (!File.Exists(certificatePath))
            return null;

        return new X509Certificate2(
            certificatePath,
            options.Value.Password ?? string.Empty,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet
        );
    }
}