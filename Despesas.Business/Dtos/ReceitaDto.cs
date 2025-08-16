using Despesas.Business.Dtos.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Despesas.Business.Dtos;
public class ReceitaDto : BaseReceitaDto
{
    [Required(ErrorMessage = "O campo Data é obrigatório.")]
    public override DateTime? Data { get; set; }

    [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
    public override string? Descricao { get; set; }

    [Required(ErrorMessage = "O campo Valor é obrigatório.")]
    public override decimal Valor { get; set; }

    [JsonIgnore]
    public override Guid? IdCategoria
    {
        get
        {
            return Categoria?.Id;
        }
        set
        {
            if (value != null && Categoria != null)
            {
                Categoria.Id = value.Value;
            }
        }
    }

    [Required(ErrorMessage = "A Categoria é obrigatória.")]
    public CategoriaDto? Categoria { get; set; } = new();
}