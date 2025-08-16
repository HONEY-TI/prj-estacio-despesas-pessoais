using Despesas.Business.Dtos.Core;

namespace Despesas.Business.Dtos.Abstractions;
public abstract class BaseDespesaDto : BaseModelDto
{
    public virtual DateTime? Data { get; set; }
    public virtual string? Descricao { get; set; }
    public virtual decimal Valor { get; set; }
    public virtual DateTime? DataVencimento { get; set; }
    public virtual Guid? IdCategoria { get; set; }
}