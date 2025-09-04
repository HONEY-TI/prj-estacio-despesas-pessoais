using Despesas.Application.Dtos.Core;

namespace Despesas.Application.Dtos;
public class LancamentoDto : BaseDto
{
    public Guid? IdDespesa { get; set; }
    public Guid? IdReceita { get; set; }
    public decimal Valor { get; set; }
    public string? Data { get; set; }
    public string? Descricao { get; set; }
    public string? TipoCategoria { get; set; }
    public string? Categoria { get; set; }
}