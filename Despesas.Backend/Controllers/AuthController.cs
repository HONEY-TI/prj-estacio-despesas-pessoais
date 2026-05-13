using Despesas.Backend.Controllers.Abstractions;
using Domain.Core.ValueObject;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Despesas.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : UnitControllerBase
{
    public AuthController() : base() { }
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