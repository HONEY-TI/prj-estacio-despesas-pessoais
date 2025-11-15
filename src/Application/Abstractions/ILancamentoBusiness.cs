namespace Application.Abstractions;
public interface ILancamentoBusiness<Dto> where Dto : class, new()
{
    Task<List<Dto>> FindByMesAno(DateTime data, Guid idUsuario);
}
