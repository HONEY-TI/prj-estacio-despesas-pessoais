using System.Text.Json.Serialization;

namespace Despesas.Application.Dtos.Core;
public abstract class BaseDto
{
    public virtual Guid? Id { get; set; }

    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
