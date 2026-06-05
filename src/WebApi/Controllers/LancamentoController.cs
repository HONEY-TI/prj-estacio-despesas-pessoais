using Application.Abstractions;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class LancamentoController : AuthController
{
    private ILancamentoBusiness<LancamentoDto> _lancamentoBusiness;
    public LancamentoController(ILancamentoBusiness<LancamentoDto> lancamentoBusiness)
    {
        _lancamentoBusiness = lancamentoBusiness;
    }

    [HttpGet("{anoMes}")]
    [Authorize("Bearer", Roles = "User, Admin")]
    [ProducesResponseType(200, Type = typeof(List<LancamentoDto>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get([FromRoute] DateTime anoMes)
    {
        try
        {
            var list = await _lancamentoBusiness.FindByMesAno(anoMes, UserIdentity);
            if (list == null || list.Count == 0)
                return Ok(new List<LancamentoDto>());

            return Ok(list);
        }
        catch
        {
            return Ok(new List<LancamentoDto>());
        }
    }
}