namespace GlobalException.CustomExceptions;

public class CategoriaUsuarioInvalidaException: ArgumentException
{
    public CategoriaUsuarioInvalidaException(): base("Categoria inválida para este usuário!") { }
}
