using Despesas.Application.Dtos;
using Despesas.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Despesas.Application.Dtos.Core;

namespace Despesas.Backend.Controllers;

public class AcessoController : AuthController
{
    private IAcessoBusiness<AcessoDto, LoginDto> _acessoBusiness;
    public AcessoController(IAcessoBusiness<AcessoDto, LoginDto> acessoBusiness)
    {
        _acessoBusiness = acessoBusiness;
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> Post([FromBody] AcessoDto acessoDto)
    {
        try
        {
            await _acessoBusiness.Create(acessoDto);
            return Ok(true);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Não foi possível realizar o cadastro.");
        }
    }

    [AllowAnonymous]
    [HttpPost("SignIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> SignIn([FromBody] LoginDto login)
    {
        try
        {
            var result = await _acessoBusiness.ValidateCredentials(login) ?? throw new();
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Não foi possível realizar o login do usuário.");
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpPost("SignInWithGoogle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthenticationDto authentication)
    {
        try
        {
            if (!authentication.Authenticated)
                throw  new ArgumentException("Erro ao autenticar com o Google.");

            var authResult = await _acessoBusiness.ValidateExternalCredentials(authentication);
            if (authResult == null)
                return Unauthorized("Usuário não autorizado.");

            return Ok(authResult);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao autenticar com o Google.");
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("ChangePassword")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordVM)
    {
        try
        {
            await _acessoBusiness.ChangePassword(UserIdentity, changePasswordVM.Senha ?? "");
            return Ok(true);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.");
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("RecoveryPassword")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> RecoveryPassword([FromBody] string email)
    {
        try
        {
            await _acessoBusiness.RecoveryPassword(email);
            return Ok(true);
        }
        catch
        {
            return NoContent();
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("refreshToken/{refreshToken}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Refresh([FromRoute] string refreshToken)
    {
        try
        {
            var result = await _acessoBusiness.ValidateCredentials(refreshToken) ?? throw new();
            return Ok(result);
        }
        catch
        {
            return NoContent();
        }
    }
}