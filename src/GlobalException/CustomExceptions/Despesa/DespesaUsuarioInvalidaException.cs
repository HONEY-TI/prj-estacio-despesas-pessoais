namespace GlobalException.CustomExceptions;

public class DespesaUsuarioInvalidaException : ArgumentException
{
    public DespesaUsuarioInvalidaException(): base("Despesa inválida para este usuário!") { }
}
