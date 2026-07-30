using System.Text.Json;
using FCG.Catalog.Application.Caching;
using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Jogo.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace FCG.Catalog.Application.Jogo.Services;

public class ListarJogosService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _ttl;

    public ListarJogosService(
        IJogoRepository jogoRepository,
        IDistributedCache cache,
        IConfiguration configuration)
    {
        _jogoRepository = jogoRepository;
        _cache = cache;
        var seconds = configuration.GetValue("Cache:JogosTtlSeconds", 60);
        _ttl = TimeSpan.FromSeconds(seconds);
    }

    public async Task<IReadOnlyCollection<JogoItemDto>> Execute()
    {
        var cached = await _cache.GetStringAsync(JogoCacheKeys.Todos);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<JogoItemDto>>(cached) ?? [];

        var jogos = await _jogoRepository.ObterTodos();
        var result = jogos
            .Select(x => new JogoItemDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Descricao = x.Descricao,
                Preco = x.Preco,
                Categoria = x.Categoria,
                Status = x.Status
            })
            .ToList();

        await _cache.SetStringAsync(
            JogoCacheKeys.Todos,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _ttl });

        return result;
    }
}
