using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Despesas.Backend.Controllers;

public class SaldoController : AuthController
{
    private ISaldoBusiness _saldoBusiness;
    public SaldoController(ISaldoBusiness saldoBusiness)
    {
        _saldoBusiness = saldoBusiness;
    }

    [HttpGet]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(SaldoDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult Get()
    {
        try
        {
            var saldo = _saldoBusiness.GetSaldo(UserIdentity);
            return Ok(saldo);
        }
        catch
        {
            return BadRequest("Erro ao gerar saldo!");
        }
    }

    [HttpGet("ByAno/{ano}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(SaldoDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult GetSaldoByAno([FromRoute] DateTime ano)
    {
        try
        {
            var saldo = _saldoBusiness.GetSaldoAnual(ano, UserIdentity);
            return Ok(saldo);
        }
        catch
        {
            return BadRequest("Erro ao gerar saldo!");
        }
    }

    [HttpGet("ByMesAno/{anoMes}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(SaldoDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult GetSaldoByMesAno([FromRoute] DateTime anoMes)
    {
        try
        {
            var saldo = _saldoBusiness.GetSaldoByMesAno(anoMes, UserIdentity);
            return Ok(saldo);
        }
        catch
        {
            return BadRequest("Erro ao gerar saldo!");
        }
    }
}
