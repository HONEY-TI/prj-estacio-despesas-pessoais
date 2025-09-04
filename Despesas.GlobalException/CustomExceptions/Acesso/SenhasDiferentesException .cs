namespace Despesas.GlobalException.CustomExceptions.Acesso;

public class SenhasDiferentesException : ArgumentException
{
    public SenhasDiferentesException() : base("Senha e Confirma Senha são diferentes!") { }
}
