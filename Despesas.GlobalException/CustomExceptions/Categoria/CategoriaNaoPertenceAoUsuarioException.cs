namespace Despesas.GlobalException.CustomExceptions.Categoria;

public class CategoriaNaoPertenceAoUsuarioException : ArgumentException
{
    public CategoriaNaoPertenceAoUsuarioException()
        : base("Categoria inválida para este usuário!") { }
}
