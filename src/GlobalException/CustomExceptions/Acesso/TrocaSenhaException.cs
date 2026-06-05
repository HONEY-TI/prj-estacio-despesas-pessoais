namespace GlobalException.CustomExceptions.Acesso;

public class TrocaSenhaException : Exception
{
    public TrocaSenhaException() : base("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.") { }
}
