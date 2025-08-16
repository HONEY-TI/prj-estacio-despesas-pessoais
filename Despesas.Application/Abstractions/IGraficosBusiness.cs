using Domain.Entities;

namespace Despesas.Application.Abstractions;
public interface IGraficosBusiness
{
    Grafico GetDadosGraficoByAnoByIdUsuario(Guid idUsuario, DateTime data);
}
