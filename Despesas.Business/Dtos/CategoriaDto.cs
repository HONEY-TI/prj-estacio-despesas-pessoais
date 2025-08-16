using Despesas.Business.Dtos.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Despesas.Business.Dtos;
public class CategoriaDto : BaseCategoriaDto
{
    [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
    public override string? Descricao { get; set; }

    [Required(ErrorMessage = "O campo Tipo de categoria é obrigatório.")]
    public BaseTipoCategoriaDto IdTipoCategoria { get; set; }
}