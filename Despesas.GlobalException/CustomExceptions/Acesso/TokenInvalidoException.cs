using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class TokenInvalidoException : AcessoException
{
    public TokenInvalidoException() : base("Token inválido", StatusCodes.Status204NoContent) { }
}