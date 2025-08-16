using Despesas.Business.Dtos.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Despesas.Business.Dtos;
public class ChangePasswordDto : BaseChangePasswordDto, IValidatableObject
{
    [Required(ErrorMessage = "O campo Senha é obrigatório.")]
    public override string? Senha { get; set; }

    [Required(ErrorMessage = "O campo Confirma Senha é obrigatório.")]
    public override string? ConfirmaSenha { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Senha) || string.IsNullOrWhiteSpace(Senha))
            yield return new ValidationResult("Campo Senha não pode ser em branco ou nulo!");

        if (string.IsNullOrEmpty(ConfirmaSenha) || string.IsNullOrWhiteSpace(ConfirmaSenha))
            yield return new ValidationResult("Campo Confirma Senha não pode ser em branco ou nulo!");
    }
}
