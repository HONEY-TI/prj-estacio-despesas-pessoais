namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class CadastroException : Exception
{
    public CadastroException() : base("Não foi possível realizar o cadastro.") { }
}