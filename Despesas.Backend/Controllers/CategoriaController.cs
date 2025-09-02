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
        try
        {
            var _categoria = await _categoriaBusiness.FindAll(UserIdentity);
            return Ok(_categoria);
        }
        catch
        {
            return Ok(new List<CategoriaDto>());
        }
    }

    [HttpGet("GetById/{idCategoria}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(CategoriaDto))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetById([FromRoute] Guid idCategoria)
    {
        try
        {
            CategoriaDto _categoria = await _categoriaBusiness.FindById(idCategoria, UserIdentity);
            return Ok(_categoria);
        }
        catch
        {
            return Ok(new CategoriaDto());
        }
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
    [ProducesResponseType(403)]
    public async Task<IActionResult> Post([FromBody] CategoriaDto categoria)
    {
        try
        {
            if (categoria.IdTipoCategoria == (int)TipoCategoriaDto.Todas)
                throw new ArgumentException("Nenhum tipo de Categoria foi selecionado!");

            categoria.UsuarioId = UserIdentity;
            return Ok(await _categoriaBusiness.Create(categoria));
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Não foi possível realizar o cadastro de uma nova categoria, tente mais tarde ou entre em contato com o suporte.");
        }
    }

    [HttpPut]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(CategoriaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Put([FromBody] CategoriaDto categoria)
    {
        try
        {
            if (categoria.IdTipoCategoria == TipoCategoriaDto.Todas)
                throw new ArgumentException("Nenhum tipo de Categoria foi selecionado!");

            categoria.UsuarioId = UserIdentity;
            CategoriaDto updateCategoria = await _categoriaBusiness.Update(categoria) ?? throw new();
            return Ok(updateCategoria);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao atualizar categoria!");
        }
    }

    [HttpDelete("{idCategoria}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid idCategoria)
    {
        try
        {
            CategoriaDto categoria = await _categoriaBusiness.FindById(idCategoria, UserIdentity);
            if (categoria == null || UserIdentity != categoria.UsuarioId)
                throw new ArgumentException("Usuário não permitido a realizar operação!");

            if (await _categoriaBusiness.Delete(categoria))
                return Ok(true);

            return Ok(false);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException argEx)
                return BadRequest(argEx.Message);

            return BadRequest("Erro ao deletar categoria!");
        }
    }
}