using Despesas.Business.Dtos.Core;

namespace Despesas.Business.Dtos.Abstractions;
public abstract class BaseCategoriaDto : BaseModelDto
{
    public virtual string? Descricao { get; set; }
}