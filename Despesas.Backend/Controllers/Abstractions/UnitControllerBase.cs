using Domain.Core.ValueObject;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Despesas.Backend.Controllers.Abstractions;


[ApiController]
public abstract class UnitControllerBase : ControllerBase
{
    protected UnitControllerBase() { }
    protected Guid UserIdentity
    {
        get
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = HttpContext.Request.Headers.Authorization.ToString();
                var jwtToken = tokenHandler.ReadToken(token.Replace("Bearer ", "")) as JwtSecurityToken;
                var userId = jwtToken?.Claims?.FirstOrDefault(c => c.Type == "sub")?.Value;
                return new Guid(userId);
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }

    protected PerfilUsuario PerfilUsuario
    {
        get
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = HttpContext.Request.Headers.Authorization.ToString();
                var jwtToken = tokenHandler.ReadToken(token.Replace("Bearer ", "")) as JwtSecurityToken;
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                if (roleClaim == null) throw new ArgumentNullException();

                if (!Enum.TryParse<PerfilUsuario.Perfil>(roleClaim, out var perfilEnum))
                    throw new ArgumentNullException();

                return new PerfilUsuario(perfilEnum);
            }
            catch
            {
                return null;
            }

        }
    }
}