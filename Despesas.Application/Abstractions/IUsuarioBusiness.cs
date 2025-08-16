using Despesas.Application.Dtos.Abstractions;

namespace Despesas.Application.Abstractions;
public interface IUsuarioBusiness<Dto> where Dto : BaseUsuarioDto, new()
{
    Dto Create(Dto dto);
    Dto FindById(Guid id);
    List<Dto> FindAll(Guid idUsuario);
    Dto Update(Dto dto);
    bool Delete(Dto dto);
}
