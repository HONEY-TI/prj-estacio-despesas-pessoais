namespace Despesas.GlobalException.CustomExceptions.Categoria;

public class TipoCategoriaInvalidaException : ArgumentException
{
    public TipoCategoriaInvalidaException(): base("Tipo de Categoria Inválida!") { }
}
