using Application.Abstractions;
using Domain.Entities;
using Repository.Persistency.Abstractions;

namespace Application.Implementations;
public class GraficosBusinessImpl : IGraficosBusiness
{
    private readonly IGraficosRepositorio _repositorio;

    public GraficosBusinessImpl(IGraficosRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Grafico> GetDadosGraficoByAnoByIdUsuario(Guid idUsuario, DateTime data)
    {
        return await _repositorio.GetDadosGraficoByAno(idUsuario, data);
    }
}
