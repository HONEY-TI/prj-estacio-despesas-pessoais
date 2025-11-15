using Application.Authentication.Abstractions;
using Business.Authentication.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Application.Authentication;

public class SigningConfigurations : ISigningConfigurations
{
    public SecurityKey? Key { get; }
    public TokenConfiguration? TokenConfiguration { get; private set; }
    public SigningCredentials? SigningCredentials { get; private set; }

    public SigningConfigurations(X509Certificate2? x509Certificate, IOptions<TokenOptions> options) : this(options)
    {
        RSA? rsa = x509Certificate?.GetRSAPrivateKey()
                ?? x509Certificate?.GetRSAPublicKey()
                ?? RSA.Create(2048);

        var rsaKey = new RsaSecurityKey(rsa)
        {
            KeyId = Guid.NewGuid().ToString()
        };

        Key = rsaKey;
        SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256);
    }

    private SigningConfigurations(IOptions<TokenOptions> options)
    {
        TokenConfiguration = new TokenConfiguration(options);
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment == Environments.Production) return;

        if (!String.IsNullOrEmpty(options.Value.Certificate) && (environment != "Test") && ((environment != Environments.Staging)))
        {
            string certificatePath = Path.Combine(AppContext.BaseDirectory, options.Value.Certificate);
            X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, options.Value.Password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            RSA? rsa = null;
            rsa = certificate.GetRSAPublicKey();
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(rsa);
            rsaSecurityKey.KeyId = Guid.NewGuid().ToString();
            SigningCredentials signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256Signature);
            Key = rsaSecurityKey;
            SigningCredentials = signingCredentials;
        }
        else
        {
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
                SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
            }
        }
    }

    public string CreateAccessToken(ClaimsIdentity identity)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = TokenConfiguration.Issuer,
            Audience = TokenConfiguration.Audience,
            SigningCredentials = SigningCredentials,
            Subject = identity,
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(TokenConfiguration.Seconds),
        });

        string token = handler.WriteToken(securityToken);
        return token;
    }

    public string GenerateRefreshToken()
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Audience = TokenConfiguration.Audience,
            Issuer = TokenConfiguration.Issuer,
            Claims = new Dictionary<string, object> { { "KEY", Guid.NewGuid() } },
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(TokenConfiguration.DaysToExpiry),
        });

        return handler.WriteToken(securityToken);
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(refreshToken.Replace("Bearer ", "")) as JwtSecurityToken;
        return jwtToken?.ValidTo >= DateTime.UtcNow;
    }
}