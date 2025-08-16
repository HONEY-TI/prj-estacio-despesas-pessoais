namespace Despesas.Business.Dtos.Abstractions;
public abstract class BaseChangePasswordDto
{
    public virtual string? Senha { get; set; }
    public virtual string? ConfirmaSenha { get; set; }
}
