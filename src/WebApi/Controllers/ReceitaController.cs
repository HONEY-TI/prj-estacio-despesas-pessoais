using Application.Abstractions;
using Application.Dtos;
using GlobalException.CustomExceptions.Core;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class ReceitaController : AuthController
{
    private readonly IBusinessBase<ReceitaDto, Receita> _receitaBusiness;
    public ReceitaController(IBusinessBase<ReceitaDto, Receita> receitaBusiness)
    {
        _receitaBusiness = receitaBusiness;
    }

    [HttpGet]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(IList<ReceitaDto>))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get()
    {
        return Ok(await _receitaBusiness.FindAll(UserIdentity));
    }

    [HttpGet("GetById/{id}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(ReceitaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var _receita = await _receitaBusiness.FindById(id, UserIdentity) 
            ?? throw new CustomException("Nenhuma receita foi encontrada.");
        return Ok(_receita);
    }

    [HttpPost]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(ReceitaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Post([FromBody] ReceitaDto receita)
    {
        receita.UsuarioId = UserIdentity;
        receita = await _receitaBusiness.Create(receita)
            ?? throw new CustomException("Não foi possível realizar o cadastro da receita!");
        return Ok(receita);
    }

    [HttpPut]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(ReceitaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Put([FromBody] ReceitaDto receita)
    {
        receita.UsuarioId = UserIdentity;
        receita = await _receitaBusiness.Update(receita)
            ?? throw new CustomException("Não foi possível atualizar o cadastro da receita.");
        return Ok(receita);
    }

    [HttpDelete("{idReceita}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid idReceita)
    {

        var despesa = new ReceitaDto
        {
            Id = idReceita,
            UsuarioId = UserIdentity
        };

        return await _receitaBusiness.Delete(despesa) ? Ok(true) : BadRequest("Erro ao excluir Receita!");
    }
}