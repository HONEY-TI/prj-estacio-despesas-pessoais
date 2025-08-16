using Despesas.Business.Dtos.Core;

namespace Despesas.Business.Dtos.Abstractions;
public abstract class BaseLancamentoDto : BaseModelDto
{
    public virtual Guid IdDespesa { get; set; }
    public virtual Guid IdReceita { get; set; }
    public virtual decimal Valor { get; set; }
    public virtual string? Data { get; set; }
    public virtual string? Descricao { get; set; }
    public virtual string? TipoCategoria { get; set; }
    public virtual string? Categoria { get; set; }
}