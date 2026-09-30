using FCG.Catalog.Application.Jogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.API.Controllers;

[ApiController]
[Authorize]
public class SearchController : ControllerBase
{
    /// <summary>
    /// Busca avançada com Fuzzy Search e ordenação por relevância (OpenSearch).
    /// Rotas: GET /api/Search?q=... e GET /search?q=...
    /// </summary>
    [HttpGet("api/Search")]
    [HttpGet("search")]
    public async Task<IActionResult> Buscar(
        [FromServices] BuscarJogosService service,
        [FromQuery] string q)
    {
        var result = await service.Execute(q);
        return Ok(result);
    }
}
