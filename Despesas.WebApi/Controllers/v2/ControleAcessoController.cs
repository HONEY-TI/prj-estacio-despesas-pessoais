using Business.Abstractions;
using Business.Dtos.Core;
using Business.Dtos.v2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Despesas.WebApi.Controllers.v2;

public class ControleAcessoController : AuthController
{
    private IControleAcessoBusiness<ControleAcessoDto, LoginDto> _controleAcessoBusiness;
    public ControleAcessoController(IControleAcessoBusiness<ControleAcessoDto, LoginDto> controleAcessoBusiness)
    {
        _controleAcessoBusiness = controleAcessoBusiness;
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    public IActionResult Post([FromBody] ControleAcessoDto controleAcessoDto)
    {
        try
        {
            _controleAcessoBusiness.Create(controleAcessoDto);
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
    [ProducesResponseType(200, Type = typeof(AuthenticationDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    public IActionResult SignIn([FromBody] LoginDto login)
    {
        try
        {
            var result = _controleAcessoBusiness.ValidateCredentials(login) ?? throw new();
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Não foi possível realizar o login do usuário.");
        }
    }

    [AllowAnonymous]
    [HttpGet("GoogleSignIn")]
    public async Task<IActionResult> GoogleSignIn()
    {
        var redirectUrl = Url.Action("GoogleCallback", "ControleAcesso", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }


    [HttpGet("GoogleCallback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync();

        if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            return BadRequest("Erro ao autenticar com o Google.");

        var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var sub = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null || sub == null)
            return BadRequest("Erro ao obter informações do Google.");

        var loginDto = new LoginDto
        {
            Email = email,
            Senha = null,
            ExternalProvider = "Google",
            ExternalId = sub
        };

        var authResult = _controleAcessoBusiness.ValidateExternalCredentials(loginDto);
        if (authResult == null)
            return Unauthorized("Usuário não autorizado.");

        return Ok(authResult);
    }

    [HttpPost("ChangePassword")]
    [Authorize("Bearer", Roles = "User")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordVM)
    {
        try
        {
            _controleAcessoBusiness.ChangePassword(UserIdentity, changePasswordVM.Senha ?? "");
            return Ok(true);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao trocar senha tente novamente mais tarde ou entre em contato com nosso suporte.");
        }
    }

    [HttpPost("RecoveryPassword")]
    [Authorize("Bearer", Roles = "User")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult RecoveryPassword([FromBody] string email)
    {
        try
        {
            _controleAcessoBusiness.RecoveryPassword(email);
            return Ok(true);
        }
        catch
        {
            return NoContent();
        }
    }

    [HttpGet("refresh/{refreshToken}")]
    [Authorize("Bearer", Roles = "User")]
    [ProducesResponseType(200, Type = typeof(AuthenticationDto))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(403)]
    public IActionResult Refresh([FromRoute] string refreshToken)
    {
        try
        {
            var result = _controleAcessoBusiness.ValidateCredentials(refreshToken) ?? throw new();
            return Ok(result);
        }
        catch
        {
            return NoContent();
        }
    }
}