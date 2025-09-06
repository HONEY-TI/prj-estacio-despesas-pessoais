namespace Despesas.GlobalException.CustomExceptions.Categoria;

public class CategoriaNaoAtualizadaException : ArgumentException
{
    public CategoriaNaoAtualizadaException()
        : base("Não foi possível atualizar a categoria, tente novamente mais tarde ou entre em contato com o suporte.") { }
}
