using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class UsuarioNaoAutorizadoException : AcessoException
{
    public UsuarioNaoAutorizadoException() : base("Usuário não autorizado", StatusCodes.Status401Unauthorized) { }
}