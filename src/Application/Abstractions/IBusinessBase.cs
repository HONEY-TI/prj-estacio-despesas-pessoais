namespace Application.Abstractions;
public interface IBusinessBase<Dto, Entity> where Dto : class where Entity : class, new()
{
    Task<Dto> Create(Dto usuarioDto);

    Task<Dto> FindById(Guid id, Guid idUsuario);
    Task<Dto> FindById(Guid id);

    Task<List<Dto>> FindAll(Guid idUsuario);

    Task<Dto> Update(Dto usuario);

    Task<bool> Delete(Dto usuario);
}