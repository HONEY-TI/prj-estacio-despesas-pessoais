namespace Despesas.Application.Abstractions;
public interface ILancamentoBusiness<Dto> where Dto : class, new()
{
    List<Dto> FindByMesAno(DateTime data, Guid idUsuario);
}
