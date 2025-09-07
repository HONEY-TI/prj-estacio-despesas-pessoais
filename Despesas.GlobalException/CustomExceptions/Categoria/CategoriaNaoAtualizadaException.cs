namespace Despesas.GlobalException.CustomExceptions.Categoria;

public class CategoriaUpdateException : ArgumentException
{
    public CategoriaUpdateException(): 
        base("Não foi possível atualizar a categoria, tente novamente mais tarde ou entre em contato com o suporte.") { }
}
