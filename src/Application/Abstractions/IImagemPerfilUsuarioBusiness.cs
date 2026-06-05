namespace Application.Abstractions;
public interface IImagemPerfilUsuarioBusiness<Dto, DtoUsuario> where Dto : class where DtoUsuario : class, new()
{
    Task<Dto> Create(Dto obj);
    Task<Dto> FindById(Guid id, Guid idUsuario);
    Task<List<Dto>> FindAll(Guid idUsuario);
    Task<Dto> Update(Dto obj);
    Task<bool> Delete(Guid idUsuario);
}
