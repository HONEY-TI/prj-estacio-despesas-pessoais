using Despesas.Application.Dtos.Core;
using System.ComponentModel.DataAnnotations;

namespace Despesas.Application.Dtos;
public class CategoriaDto : BaseDto
{
    [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O campo Tipo de categoria é obrigatório.")]
    public TipoCategoriaDto IdTipoCategoria { get; set; }
}