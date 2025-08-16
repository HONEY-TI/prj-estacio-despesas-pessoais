using Despesas.Application.Dtos.Abstractions;

namespace Despesas.Application.Dtos;

public class GoogleAuthenticationDto : BaseAuthenticationDto
{
    public virtual string? Nome { get; set; }
    public virtual string? SobreNome { get; set; }
    public virtual string? Telefone { get; set; }
    public virtual string? Email { get; set; }
    public string? RefreshTokenExpiry { get; set; }    
    public string? ExternalProvider { get; set; }
    public string? ExternalId { get; set; }
}
