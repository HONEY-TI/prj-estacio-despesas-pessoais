namespace Despesas.Application.Abstractions;
public interface IImagemPerfilUsuarioBusiness<Dto, DtoUsuario> where Dto : class where DtoUsuario : class, new()
{
    Dto Create(Dto obj);
    Dto FindById(Guid id, Guid idUsuario);
    List<Dto> FindAll(Guid idUsuario);
    Dto Update(Dto obj);
    bool Delete(Guid idUsuario);
}
