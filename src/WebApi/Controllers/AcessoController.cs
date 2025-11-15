using Application.Abstractions;
using Application.Dtos;
using Application.Dtos.Core;
using WebApi.Controllers.Abstractions;
using GlobalException.CustomExceptions.Acesso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class AcessoController : UnitControllerBase
{
    private readonly IAcessoBusiness<AcessoDto, LoginDto> _acessoBusiness;

    public AcessoController(IAcessoBusiness<AcessoDto, LoginDto> acessoBusiness)
    {
        _acessoBusiness = acessoBusiness;
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(200, Type = typeof(bool))]
    public async Task<IActionResult> Post([FromBody] AcessoDto acessoDto)
    {
        await _acessoBusiness.Create(acessoDto);
        return Ok(true);
    }

    [AllowAnonymous]
    [HttpPost("SignIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SignIn([FromBody] LoginDto login)
    {
        var result = await _acessoBusiness.ValidateCredentials(login);
        return Ok(result);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpPost("SignInWithGoogle")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthenticationDto authentication)
    {
        if (!authentication.Authenticated)
            throw new ArgumentException("Erro ao autenticar com o Google.");

        var authResult = await _acessoBusiness.ValidateExternalCredentials(authentication);
        return Ok(authResult);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("ChangePassword")]
    [Authorize("Bearer", Roles = "User, Admin")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto == null)
            throw new TrocaSenhaException();

        await _acessoBusiness.ChangePassword(UserIdentity, changePasswordDto.Senha ?? "");
        return Ok(true);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("RecoveryPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> RecoveryPassword([FromBody] string email)
    {
        await _acessoBusiness.RecoveryPassword(email);
        return Ok(true);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("refreshToken/{refreshToken}")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromRoute] string refreshToken)
    {
        var result = await _acessoBusiness.ValidateCredentials(refreshToken);
        return Ok(result);
    }
}