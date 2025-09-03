using Domain.Entities;
namespace Repository.Persistency.Abstractions;
public interface ILancamentoRepositorio
{
    Task<ICollection<Lancamento>> FindByMesAno(DateTime data, Guid idUsuario);
}
