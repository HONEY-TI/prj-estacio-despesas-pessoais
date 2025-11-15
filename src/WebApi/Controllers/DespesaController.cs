using Application.Abstractions;
using Application.Dtos;
using GlobalException.CustomExceptions.Core;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
public class DespesaController : AuthController
{
    private readonly IBusinessBase<DespesaDto, Despesa> _despesaBusiness;
    public DespesaController(IBusinessBase<DespesaDto, Despesa> despesaBusiness)
    {
        _despesaBusiness = despesaBusiness;
    }

    [HttpGet]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(IList<DespesaDto>))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get()
    {
        return Ok(await _despesaBusiness.FindAll(UserIdentity));
    }

    [HttpGet("GetById/{id}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(DespesaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var despesa = await _despesaBusiness.FindById(id, UserIdentity) 
            ?? throw new CustomException("Nenhuma despesa foi encontrada.");
        return Ok(despesa);
    }

    [HttpPost]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(DespesaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Post([FromBody] DespesaDto despesa)
    {
        despesa.UsuarioId = UserIdentity;
        despesa = await _despesaBusiness.Create(despesa)
            ?? throw new CustomException("Não foi possível realizar o cadastro da despesa.");
        return Ok(despesa);
    }

    [HttpPut]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(DespesaDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Put([FromBody] DespesaDto despesa)
    {
        despesa.UsuarioId = UserIdentity;
        despesa = await _despesaBusiness.Update(despesa) 
            ?? throw new CustomException("Não foi possível atualizar o cadastro da despesa.");
        return Ok(despesa);
    }

    [HttpDelete("{idDespesa}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid idDespesa)
    {
        var despesa = new DespesaDto
        {
            Id = idDespesa,
            UsuarioId = UserIdentity
        };

        return await _despesaBusiness.Delete(despesa) ? Ok(true) : BadRequest("Erro ao excluir Despesa!");
   }
}