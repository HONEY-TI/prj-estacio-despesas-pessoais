namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class UsuarioInexistenteException : ArgumentException
{
    public UsuarioInexistenteException() : base("Usuário Inexistente!") { }
}