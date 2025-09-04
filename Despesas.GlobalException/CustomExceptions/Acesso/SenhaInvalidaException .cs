using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class SenhaInvalidaException : AcessoException
{
    public SenhaInvalidaException() : base("Senha inválida!", StatusCodes.Status400BadRequest) { }
}