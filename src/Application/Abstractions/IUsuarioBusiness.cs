using Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions;
public interface IUsuarioBusiness<Dto> where Dto : UsuarioDto, new()
{
    Task<Dto> Create(Dto dto);
    Task<Dto> FindById(Guid id);
    Task<List<Dto>> FindAll(Guid idUsuario);
    Task<Dto> Update(Dto dto);
    Task<bool> Delete(Dto dto);
    Task<byte[]> GetProfileImage(Guid userIdentity);
    Task<byte[]> UpdateProfileImage(Guid userIdentity, IFormFile file);
}
