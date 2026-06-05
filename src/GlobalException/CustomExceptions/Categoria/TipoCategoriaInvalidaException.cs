namespace GlobalException.CustomExceptions;

public class TipoCategoriaInvalidaException : ArgumentException
{
    public TipoCategoriaInvalidaException(): base("Tipo de Categoria Inválida!") { }
}
