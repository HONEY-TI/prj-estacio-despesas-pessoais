namespace Despesas.GlobalException.CustomExceptions.Categoria;

public class CategoriaUsuarioInvalidaException: ArgumentException
{
    public CategoriaUsuarioInvalidaException(): base("Categoria inválida para este usuário!") { }
}
