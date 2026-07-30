using System.Security.Claims;
using FCG.Catalog.Application.Avaliacao.Dtos;
using FCG.Catalog.Application.Avaliacao.Services;
using FCG.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AvaliacaoController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromServices] CriarAvaliacaoService service,
        [FromBody] CriarAvaliacaoDto.Request request)
    {
        var userId = ObterUsuarioId();
        var id = await service.Execute(userId, request);
        return CreatedAtAction(nameof(ListarPorJogo), new { jogoId = request.JogoId }, new { id });
    }

    [HttpGet("jogo/{jogoId:guid}")]
    public async Task<IActionResult> ListarPorJogo(
        [FromServices] ListarAvaliacoesPorJogoService service,
        [FromRoute] Guid jogoId)
    {
        var result = await service.Execute(jogoId);
        return Ok(result);
    }

    private Guid ObterUsuarioId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var usuarioId))
            throw new DomainException("Token inválido.");

        return usuarioId;
    }
}
