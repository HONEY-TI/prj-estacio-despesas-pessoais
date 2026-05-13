namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class EmailInvalidoException : ArgumentException
{
    public EmailInvalidoException() : base("Email inválido!") { }
}