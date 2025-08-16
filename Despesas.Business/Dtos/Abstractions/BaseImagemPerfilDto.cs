using Despesas.Business.Dtos.Core;

namespace Despesas.Business.Dtos.Abstractions;
public abstract class BaseImagemPerfilDto : BaseModelDto
{
    public virtual string? Url { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Type { get; set; }
    public virtual string? ContentType { get; set; }
    public virtual byte[]? Arquivo { get; set; }
}