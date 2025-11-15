using Domain.Entities;

namespace Application.Abstractions;
public interface IGraficosBusiness
{
    Task<Grafico> GetDadosGraficoByAnoByIdUsuario(Guid idUsuario, DateTime data);
}
