namespace GlobalException.CustomExceptions.Acesso;

public class LoginException : ArgumentException
{
    public LoginException() : base("Não foi possível realizar o login do usuário.") { }
}