using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Despesas.Backend.Controllers;

public class UsuarioController : AuthController
{
    private readonly IUsuarioBusiness<UsuarioDto> _usuarioBusiness;    

    public UsuarioController(IUsuarioBusiness<UsuarioDto> usuarioBusiness)
    {
        _usuarioBusiness = usuarioBusiness;
    
    }

    [HttpGet]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(UsuarioDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var _usuario = _usuarioBusiness.FindById(UserIdentity);
            if (_usuario == null) throw new();
            return Ok(_usuario);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Usuário não encontrado!");
        }
    }

    [HttpPost]
    [Authorize("Bearer", Roles = "Admin")]
    [ProducesResponseType(200, Type = typeof(UsuarioDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Post([FromBody] UsuarioDto usuarioDto)
    {
        try
        {
            usuarioDto.UsuarioId = UserIdentity;
            return Ok(_usuarioBusiness.Create(usuarioDto));
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao cadastrar Usuário!");
        }
    }

    [HttpPut]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(UsuarioDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Put([FromBody] UsuarioDto usuarioDto)
    {
        try
        {
            usuarioDto.UsuarioId = UserIdentity;
            return Ok(_usuarioBusiness.Update(usuarioDto));
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao atualizar dados pessoais do usuário!");
        }
    }    
    
    [HttpDelete]
    [Authorize("Bearer", Roles = "Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete([FromBody] UsuarioDto usuarioDto)
    {
        try
        {
            if (_usuarioBusiness.Delete(usuarioDto))
                return Ok(true);

            throw new ArgumentException("Não foi possivél excluir este usuário.");
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao excluir Usuário!");
        }
    }

    [HttpGet("GetProfileImage")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetProfileImage()
    {
        try
        {
            var image = _usuarioBusiness.GetProfileImage(UserIdentity);

            if(image == null || image.Length == 0)
                return NoContent();

            return File(image!, "image/png");
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);
            
            return BadRequest("Erro ao incluir imagem de perfil!");
        }
    }

    [HttpPut("UpdateProfileImage")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> PutProfileImage(IFormFile file)
    {
        try
        {
            var image = _usuarioBusiness.UpdateProfileImage(UserIdentity, file);

            if (image == null || image.Length == 0)
                return NoContent();

            return File(image!, file.ContentType);

        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao incluir imagem de perfil!");
        }
    }
}