using Despesas.Application.Dtos.Core;

namespace Despesas.Application.Dtos.Abstractions;
public abstract class BaseCategoriaDto : BaseModelDto
{
    public virtual string? Descricao { get; set; }
}