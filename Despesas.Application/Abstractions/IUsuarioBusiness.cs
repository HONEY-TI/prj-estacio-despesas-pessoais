using Despesas.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Despesas.Application.Abstractions;
public interface IUsuarioBusiness<Dto> where Dto : UsuarioDto, new()
{
    Dto Create(Dto dto);
    Dto FindById(Guid id);
    List<Dto> FindAll(Guid idUsuario);
    Dto Update(Dto dto);
    bool Delete(Dto dto);    
    byte[] GetProfileImage(Guid userIdentity);
    byte[] UpdateProfileImage(Guid userIdentity, IFormFile file);
}
