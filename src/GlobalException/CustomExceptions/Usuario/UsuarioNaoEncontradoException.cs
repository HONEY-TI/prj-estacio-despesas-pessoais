namespace GlobalException.CustomExceptions;

public class UsuarioNaoEncontradoException : ArgumentException
{
    public UsuarioNaoEncontradoException(): base("Usuário não encontrado!") { }
}
