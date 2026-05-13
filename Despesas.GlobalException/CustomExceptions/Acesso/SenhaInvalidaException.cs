using Despesas.GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class SenhaInvalidaException : CustomException
{
    public SenhaInvalidaException() : base("Senha inválida!", StatusCodes.Status400BadRequest) { }
}