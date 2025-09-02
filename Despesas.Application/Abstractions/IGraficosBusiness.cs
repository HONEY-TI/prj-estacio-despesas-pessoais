using Domain.Entities;

namespace Despesas.Application.Abstractions;
public interface IGraficosBusiness
{
    Task<Grafico> GetDadosGraficoByAnoByIdUsuario(Guid idUsuario, DateTime data);
}
