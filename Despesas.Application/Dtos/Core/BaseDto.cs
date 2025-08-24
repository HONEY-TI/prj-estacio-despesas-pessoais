using System.Text.Json.Serialization;

namespace Despesas.Application.Dtos.Core;
public abstract class BaseDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
