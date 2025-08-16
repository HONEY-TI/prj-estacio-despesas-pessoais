using Despesas.Backend.Controllers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Despesas.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : UnitControllerBase
{
    public AuthController() : base() { }
}