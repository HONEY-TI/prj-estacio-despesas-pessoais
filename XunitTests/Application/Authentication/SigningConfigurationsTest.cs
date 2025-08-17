using Despesas.Application.Authentication;
using Despesas.Business.Authentication.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Application.Authentication;
public sealed class SigningConfigurationsTest
{
    private IOptions<TokenOptions> options;
    public SigningConfigurationsTest()
    {
        options = Options.Create(new TokenOptions
        {
            Issuer = "testIssuer",
            Audience = "testAudience",
            Seconds = 30,
            Certificate = "certificate/webapi-cert.pfx",
            Password = "12345T!"
        });
    }

    [Fact]
    public void SigningConfigurations_Should_Initialize_Correctly()
    {
        // Arrange

        // Act
        var signingConfigurations = new SigningConfigurations(Usings.GenerateCertificate(), options);

        // Assert
        Assert.NotNull(signingConfigurations.Key);
        Assert.NotNull(signingConfigurations.SigningCredentials);
    }

    [Fact]
    public void SigningConfigurations_Private_Should_Initialize_Correctly()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        var builder = WebApplication.CreateBuilder();
        string certificatePath = Path.Combine(AppContext.BaseDirectory, options.Value.Certificate);
        X509Certificate2 certificate = new X509Certificate2(certificatePath, options.Value.Password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
        
        // Act
        var signingConfigurations = new SigningConfigurations(certificate, options);
        builder.Services.AddSingleton<SigningConfigurations>(signingConfigurations);


        // Assert
        Assert.NotNull(signingConfigurations.Key);
        Assert.NotNull(signingConfigurations.SigningCredentials);
    }

    [Fact]
    public void Key_Should_Be_RSA_SecurityKey()
    {

        // Act
        var signingConfigurations = new SigningConfigurations(Usings.GenerateCertificate(), options);

        // Assert
        Assert.IsType<RsaSecurityKey>(signingConfigurations.Key);
    }

    [Fact]
    public void SigningCredentials_Should_Be_Correct_Algorithm()
    {

        // Act
        var signingConfigurations = new SigningConfigurations(Usings.GenerateCertificate(), options);

        // Assert
        Assert.NotNull(signingConfigurations.SigningCredentials.Algorithm);
        Assert.Equal(SecurityAlgorithms.RsaSha256, signingConfigurations.SigningCredentials.Algorithm);
    }

    [Fact]
    public void CreateAccessToken_Should_Generate_Valid_Token()
    {
        // Arrange
        var options = Options.Create(new TokenOptions
        {
            Issuer = "testIssuer",
            Audience = "testAudience",
            Seconds = 3600 // 1 hour expiration for testing
        });

        var signingConfigurations = new SigningConfigurations(Usings.GenerateCertificate(), options); var handler = new JwtSecurityTokenHandler();
        var userId = Guid.NewGuid();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testUser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(JwtRegisteredClaimNames.UniqueName, "testUser"),
            new Claim(JwtRegisteredClaimNames.UniqueName, "Customer")
        });

        // Act
        var token = signingConfigurations.CreateAccessToken(claimsIdentity);

        // Assert
        Assert.NotNull(token);
        Assert.True(handler.CanReadToken(token));

        var validatedToken = handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Value.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingConfigurations.Key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out _);

        Assert.NotNull(validatedToken);
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_Valid_JWT()
    {
        // Arrange
        var signingConfigurations = new SigningConfigurations(Usings.GenerateCertificate(), options);
        var handler = new JwtSecurityTokenHandler();

        // Act
        var refreshToken = signingConfigurations.GenerateRefreshToken();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));
        Assert.True(handler.CanReadToken(refreshToken));
    }
}