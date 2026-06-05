using GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Http;

namespace GlobalException.CustomExceptions.Acesso;

public class UsuarioInativoException : CustomException
{
    public UsuarioInativoException() : base("Usuário Inativo!", StatusCodes.Status401Unauthorized) { }
}
