namespace Despesas.Business.Authentication.Abstractions;

public class GoogleOAuthOptions
{
    public string? ClientId { get; set; }
    public string? ProjectId { get; set; }
    public string? AuthUri { get; set; }
    public string? TokenUri { get; set; }
    public string? AuthProviderX509CertUrl { get; set; }
    public string? ClientSecret { get; set; }
    public List<string>? RedirectUris { get; set; }
    public string? CallbackPath { get; set; }
    public List<string>? JavascriptOrigins { get; set; }
}
