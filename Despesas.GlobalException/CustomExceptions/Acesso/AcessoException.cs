namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class AcessoException : Exception
{
    public int StatusCode { get; }

    public AcessoException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}
