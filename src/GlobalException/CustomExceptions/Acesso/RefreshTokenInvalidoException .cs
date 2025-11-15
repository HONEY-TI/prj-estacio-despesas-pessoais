using GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Http;

namespace GlobalException.CustomExceptions.Acesso;

public class RefreshTokenInvalidoException : CustomException
{
    public RefreshTokenInvalidoException() : base("Refresh Token Inválido!", StatusCodes.Status204NoContent) { }
}