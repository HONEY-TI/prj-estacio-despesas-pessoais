using Application.Dtos.Abstractions;

namespace Application.Dtos.Core;

public class GoogleAuthenticationDto : AuthenticationDto
{
    public virtual string? Nome { get; set; }
    public virtual string? SobreNome { get; set; }
    public virtual string? Telefone { get; set; }
    public virtual string? Email { get; set; }
    public string? ExternalProvider { get; set; }
    public string? ExternalId { get; set; }
}