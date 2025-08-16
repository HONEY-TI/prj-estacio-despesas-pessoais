using Despesas.Application.Dtos.Core;
using Domain.Entities.ValueObjects;
using System.Text.Json.Serialization;

namespace Despesas.Application.Dtos.Abstractions;

public class BaseUsuarioDto : BaseModelDto
{    
    public virtual string? Nome { get; set; }
    public virtual string? SobreNome { get; set; }
    public virtual string? Telefone { get; set; }
    public virtual string? Email { get; set; }
    
    [JsonIgnore]
    public virtual PerfilUsuario? PerfilUsuario { get; set; }
}