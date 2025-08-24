using Despesas.Application.Dtos;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos.Abstractions;
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
    public IActionResult Post([FromBody] AcessoDto acessoDto)
    {
        try
        {
            _acessoBusiness.Create(acessoDto);
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
            var result = _acessoBusiness.ValidateCredentials(login) ?? throw new();
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
    [ProducesResponseType(200, Type = typeof(AuthenticationDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthenticationDto authentication)
    {
       
        if (!authentication.Authenticated)
            return BadRequest("Erro ao autenticar com o Google.");
               
        var authResult = _acessoBusiness.ValidateExternalCredentials(authentication);
        if (authResult == null)
            return Unauthorized("Usuário não autorizado.");
           
        return Ok(authResult);
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("ChangePassword")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordVM)
    {
        try
        {
            _acessoBusiness.ChangePassword(UserIdentity, changePasswordVM.Senha ?? "");
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
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult RecoveryPassword([FromBody] string email)
    {
        try
        {
            _acessoBusiness.RecoveryPassword(email);
            return Ok(true);
        }
        catch
        {
            return NoContent();
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("refresh/{refreshToken}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(AuthenticationDto))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(403)]
    public IActionResult Refresh([FromRoute] string refreshToken)
    {
        try
        {
            var result = _acessoBusiness.ValidateCredentials(refreshToken) ?? throw new();
            return Ok(result);
        }
        catch
        {
            return NoContent();
        }
    }
}