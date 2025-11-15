using Domain.Entities;

namespace Repository.Persistency.Abstractions;
public interface IGraficosRepositorio
{
    Task<Grafico> GetDadosGraficoByAno(Guid idUsuario, DateTime data);
}
