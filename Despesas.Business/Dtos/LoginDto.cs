using Despesas.Business.Dtos.Core;
using System.ComponentModel.DataAnnotations;

namespace Despesas.Business.Dtos;
public class LoginDto : BaseLoginDto
{
    [EmailAddress(ErrorMessage = "O campo Email é inválido.")]
    [Required(ErrorMessage = "O campo Email é obrigatório.")]
    public override string? Email { get; set; }

    [Required(ErrorMessage = "O campo Senha é obrigatório.")]
    public override string? Senha { get; set; }

}