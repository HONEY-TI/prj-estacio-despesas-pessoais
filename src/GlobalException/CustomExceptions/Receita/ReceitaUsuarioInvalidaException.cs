namespace GlobalException.CustomExceptions;

public class ReceitaUsuarioInvalidaException : ArgumentException
{
    public ReceitaUsuarioInvalidaException(): base("Receita inválida para este usuário!") { }
}
