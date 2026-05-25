using System.Security.Claims;
using FCG.Catalog.Application.Biblioteca.Services;
using FCG.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BibliotecaController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromServices] ListarBibliotecaService service)
    {
        var usuarioId = ObterUsuarioId();
        var biblioteca = await service.Execute(usuarioId);
        return Ok(biblioteca);
    }

    [HttpPost("{jogoId:guid}/comprar")]
    public async Task<IActionResult> Comprar(
        [FromServices] ComprarJogoService service,
        [FromRoute] Guid jogoId)
    {
        var usuarioId = ObterUsuarioId();
        var orderId = await service.Execute(usuarioId, jogoId);
        return Accepted(new { orderId });
    }

    private Guid ObterUsuarioId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var usuarioId))
            throw new DomainException("Token inválido.");

        return usuarioId;
    }
}
