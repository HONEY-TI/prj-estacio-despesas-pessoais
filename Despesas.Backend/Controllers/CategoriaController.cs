using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Application.Dtos.Core;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Despesas.Backend.Controllers;

public class CategoriaController : AuthController
{
    private readonly ICategoriaBusiness<CategoriaDto, Categoria> _categoriaBusiness;
    public CategoriaController(ICategoriaBusiness<CategoriaDto, Categoria> categoriaBusiness)
    {
        _categoriaBusiness = categoriaBusiness;
    }

    [HttpGet]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(List<CategoriaDto>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get()
    {
        var categoria = await _categoriaBusiness.FindAll(UserIdentity);
        return Ok(categoria);
    }

    [HttpGet("GetById/{idCategoria}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(CategoriaDto))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetById([FromRoute] Guid idCategoria)
    {
        var ccategoria = await _categoriaBusiness.FindById(idCategoria, UserIdentity);
        return Ok(ccategoria);
    }

    [HttpGet("GetByTipoCategoria/{tipoCategoria}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(List<CategoriaDto>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetByTipoCategoria([FromRoute] TipoCategoriaDto tipoCategoria)
    {
        var _categoria = await _categoriaBusiness.FindByTipocategoria(UserIdentity, (int)tipoCategoria);
        return Ok(_categoria);
    }

    [HttpPost]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(CategoriaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)] public async Task<IActionResult> Post([FromBody] CategoriaDto categoria)
    {
        if (categoria.IdTipoCategoria == (int)TipoCategoriaDto.Todas)
            return BadRequest("Nenhum tipo de Categoria foi selecionado!");

        categoria.UsuarioId = UserIdentity;
        categoria = await _categoriaBusiness.Create(categoria);
        if (categoria == null)
            return BadRequest("Não foi possível realizar o cadastro de uma nova categoria, tente mais tarde ou entre em contato com o suporte.");

        return Ok(categoria);
    }

    [HttpPut]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(CategoriaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Put([FromBody] CategoriaDto categoria)
    {
        if (categoria.IdTipoCategoria == TipoCategoriaDto.Todas)
            return BadRequest("Nenhum tipo de Categoria foi selecionado!");

        categoria.UsuarioId = UserIdentity;
        categoria = await _categoriaBusiness.Update(categoria);
        if (categoria is null)
            return BadRequest("Erro ao atualizar categoria!"); ;

        return Ok(categoria);
    }

    [HttpDelete("{idCategoria}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid idCategoria)
    {
        var categoria = new CategoriaDto()
        {
            Id = idCategoria,
            UsuarioId = UserIdentity
        };
        var result = await _categoriaBusiness.Delete(categoria);
        if (result)
            return Ok(result);
        else
            return BadRequest("Erro ao deletar categoria!");

    }
}