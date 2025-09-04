using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class RefreshTokenInvalidoException : AcessoException
{
    public RefreshTokenInvalidoException() : base("Refresh Token Inválido!", StatusCodes.Status204NoContent) { }
}