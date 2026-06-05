namespace Application.Abstractions;
public interface ICategoriaBusiness<Dto, Entity> where Dto : class where Entity : class, new()
{
    Task<Dto> Create(Dto usuarioDto);

    Task<Dto> FindById(Guid id, Guid idUsuario);
    Task<Dto> FindById(Guid id);
    Task<List<Dto>> FindAll(Guid idUsuario);    
    Task<List<Dto>> FindByTipocategoria(Guid idUsuario, int IdTipoCategoria);
    Task<Dto> Update(Dto usuario);
    Task<bool> Delete(Dto usuario);
}