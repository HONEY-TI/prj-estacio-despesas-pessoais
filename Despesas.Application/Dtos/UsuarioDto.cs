using Despesas.Application.Dtos.Abstractions;
using Domain.Entities.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Despesas.Application.Dtos;

public class UsuarioDto : BaseUsuarioDto
{
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public override string? Nome { get; set; }

    public override string? SobreNome { get; set; }

    [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
    public override string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "O campo Email é inválido.")]
    [Required(ErrorMessage = "O campo Email é obrigatório.")]
    public override string? Email { get; set; }

    [JsonIgnore]
    public override PerfilUsuario? PerfilUsuario { get; set; }
}