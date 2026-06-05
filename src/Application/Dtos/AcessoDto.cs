using Application.Dtos.Core;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos;
public class AcessoDto : BaseDto, IValidatableObject
{
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string? Nome { get; set; }

    public string? SobreNome { get; set; }

    [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
    public string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "O campo Email é inválido.")]
    [Required(ErrorMessage = "O campo Email é obrigatório.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O campo Senha é obrigatório.")]
    [PasswordPropertyText]
    public string? Senha { get; set; }

    [Required(ErrorMessage = "O campo Confirma Senha é obrigatório.")]
    [PasswordPropertyText]
    public string? ConfirmaSenha { get; set; }

    [JsonIgnore]
    public string? ExternalProvider { get; set; }

    [JsonIgnore]
    public string? ExternalId { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {

        if (string.IsNullOrEmpty(ConfirmaSenha) || string.IsNullOrWhiteSpace(ConfirmaSenha))
            yield return new ValidationResult("Campo Confirma Senha não pode ser em branco ou nulo");

        if (Senha != ConfirmaSenha)
            yield return new ValidationResult("Senha e Confirma Senha são diferentes!");
    }
}