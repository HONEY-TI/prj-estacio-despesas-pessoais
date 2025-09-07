namespace Despesas.GlobalException.CustomExceptions.Categoria;

public class DespesaUsuarioInvalidaException : ArgumentException
{
    public DespesaUsuarioInvalidaException(): base("Despesa inválida para este usuário!") { }
}
