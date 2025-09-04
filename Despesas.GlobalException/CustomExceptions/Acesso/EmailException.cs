using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class EmailException : AcessoException
{
    public EmailException() : base("Erro ao enviar email de recuperação de senha!", StatusCodes.Status400BadRequest) { }
}