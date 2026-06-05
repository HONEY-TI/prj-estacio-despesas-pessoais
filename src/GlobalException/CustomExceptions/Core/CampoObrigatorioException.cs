namespace GlobalException.CustomExceptions.Core;

public class CampoObrigatorioException : ArgumentException
{
    public CampoObrigatorioException(string campo): base($"Campo {campo} não pode ser em branco ou nulo!") { }
}