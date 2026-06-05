namespace GlobalException.CustomExceptions.Core;

public class UsuarioNaoAutorizadoException : ArgumentException
{
    public UsuarioNaoAutorizadoException(): base("Usuário não permitido a realizar operação!") { }
}
