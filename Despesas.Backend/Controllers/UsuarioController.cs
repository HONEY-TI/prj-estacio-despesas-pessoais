using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.GlobalException.CustomExceptions;
using Despesas.GlobalException.CustomExceptions.Core;
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
        var usuario = await _usuarioBusiness.FindById(UserIdentity)
            ?? throw new UsuarioNaoEncontradoException();
        return Ok(usuario);
    }

    [HttpPost]
    [Authorize("Bearer", Roles = "Admin")]
    [ProducesResponseType(200, Type = typeof(UsuarioDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Post([FromBody] UsuarioDto usuarioDto)
    {
        usuarioDto.UsuarioId = UserIdentity;
        usuarioDto = await _usuarioBusiness.Create(usuarioDto)
            ?? throw new CustomException("Erro ao cadastrar Usuário!");
        return Ok(usuarioDto);
    }

    [HttpPut]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(UsuarioDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Put([FromBody] UsuarioDto usuarioDto)
    {
        usuarioDto.UsuarioId = UserIdentity;
        usuarioDto = await _usuarioBusiness.Update(usuarioDto)
            ?? throw new CustomException("Erro ao atualizar dados pessoais do usuário!");
        return Ok(usuarioDto);
    }    
    
    [HttpDelete]
    [Authorize("Bearer", Roles = "Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete([FromBody] UsuarioDto usuarioDto)
    {
        usuarioDto.UsuarioId = UserIdentity;
        return await _usuarioBusiness.Delete(usuarioDto) 
            ? Ok(true) 
            : BadRequest("Erro ao excluir Usuário!");
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
        var image = await _usuarioBusiness.GetProfileImage(UserIdentity);

        if (image == null || image.Length == 0)
            return NoContent();

        return File(image, "image/png");
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
        var image = await _usuarioBusiness.UpdateProfileImage(UserIdentity, file);

        if (image == null || image.Length == 0)
            return NoContent();

        return File(image, file.ContentType);
    }

}