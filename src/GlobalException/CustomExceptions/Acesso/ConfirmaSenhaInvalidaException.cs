namespace GlobalException.CustomExceptions.Acesso;

public class ConfirmaSenhaInvalidaException : ArgumentException
{
    public ConfirmaSenhaInvalidaException() : base("Campo Confirma Senha não pode ser em branco ou nulo!") { }
}
