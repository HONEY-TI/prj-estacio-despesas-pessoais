using Despesas.Application.Dtos.Abstractions;

namespace Despesas.Application.Dtos;
public class LancamentoDto : BaseLancamentoDto
{
    public override Guid IdDespesa { get; set; }
    public override Guid IdReceita { get; set; }
    public override decimal Valor { get; set; }
    public override string? Data { get; set; }
    public override string? Descricao { get; set; }
    public override string? TipoCategoria { get; set; }
    public override string? Categoria { get; set; }
}