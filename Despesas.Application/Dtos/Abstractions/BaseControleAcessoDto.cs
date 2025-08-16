using Despesas.Application.Dtos.Core;
using System.ComponentModel.DataAnnotations;

namespace Despesas.Application.Dtos.Abstractions;

public abstract class BaseControleAcessoDto : BaseModelDto
{
    [Required]
    public virtual string? Nome { get; set; }

    public virtual string? SobreNome { get; set; }

    [Required]
    public virtual string? Telefone { get; set; }

    [EmailAddress]
    [Required]
    public virtual string? Email { get; set; }

    [Required]
    public virtual string? Senha { get; set; }

    [Required]    
    public virtual string? ConfirmaSenha { get; set; }
    
    public virtual string? ExternalProvider { get; set; }
    
    public virtual string? ExternalId { get; set; }

}