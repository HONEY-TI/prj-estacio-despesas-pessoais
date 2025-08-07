using Despesas.Business.Authentication.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.CommonDependenceInject;

public static class GoogleAuthentication
{

    public static void AddGoogleAuthentication(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var googleOAuthOptions = new GoogleOAuthOptions();
        configuration.GetSection("GoogleOAuthConfigurations").Bind(googleOAuthOptions);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        }).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = googleOAuthOptions.ClientId!;
            options.ClientSecret = googleOAuthOptions.ClientSecret!;
            options.SaveTokens = true;
            options.CallbackPath = googleOAuthOptions.CallbackPath;
        });
    }
}
