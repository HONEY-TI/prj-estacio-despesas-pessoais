using Despesas.GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class LoginException : CustomException
{
    public LoginException() : base("Não foi possível realizar o login do usuário.", StatusCodes.Status400BadRequest) { }
}