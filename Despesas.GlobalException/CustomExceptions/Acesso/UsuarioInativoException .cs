using Microsoft.AspNetCore.Http;

namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class UsuarioInativoException : AcessoException
{
    public UsuarioInativoException() : base("Usuário Inativo!", StatusCodes.Status401Unauthorized) { }
}
