using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Application.Jogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JogoController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Listar([FromServices] ListarJogosService service)
    {
        var result = await service.Execute();
        return Ok(result);
    }

    [HttpGet("ativos")]
    [Authorize]
    public async Task<IActionResult> ListarAtivos([FromServices] ListarJogosAtivosService service)
    {
        var result = await service.Execute();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar(
        [FromServices] CriarJogoService service,
        [FromBody] CriarJogoDto.Request request)
    {
        var id = await service.Execute(request);
        return CreatedAtAction(nameof(Listar), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromServices] AtualizarJogoService service,
        [FromBody] AtualizarJogoDto.Request request)
    {
        await service.Execute(id, request);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromServices] AlterarStatusJogoService service)
    {
        await service.Execute(id);
        return NoContent();
    }
}
