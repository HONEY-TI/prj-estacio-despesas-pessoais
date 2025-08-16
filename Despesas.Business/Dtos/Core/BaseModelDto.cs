using Domain.Core;
using System.Text.Json.Serialization;

namespace Despesas.Business.Dtos.Core;
public abstract class BaseModelDto : BaseModel
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}
