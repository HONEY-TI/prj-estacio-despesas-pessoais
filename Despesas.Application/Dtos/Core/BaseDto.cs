using System.Text.Json.Serialization;

namespace Despesas.Application.Dtos.Core;
public abstract class BaseDto
{
    public virtual Guid? Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public virtual Guid UsuarioId { get; set; }
}
