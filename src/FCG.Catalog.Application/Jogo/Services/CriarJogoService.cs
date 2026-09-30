using FCG.Catalog.Application.Caching;
using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace FCG.Catalog.Application.Jogo.Services;

public class CriarJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IJogoSearchRepository _searchRepository;
    private readonly IDistributedCache _cache;

    public CriarJogoService(
        IJogoRepository jogoRepository,
        IJogoSearchRepository searchRepository,
        IDistributedCache cache)
    {
        _jogoRepository = jogoRepository;
        _searchRepository = searchRepository;
        _cache = cache;
    }

    public async Task<Guid> Execute(CriarJogoDto.Request request)
    {
        var jogo = JogoEntity.Criar(request.Nome, request.Descricao, request.Preco, request.Categoria);
        await _jogoRepository.Adicionar(jogo);
        await _jogoRepository.SalvarAlteracoes();
        await _searchRepository.Indexar(jogo);
        await InvalidarCache();
        return jogo.Id;
    }

    private async Task InvalidarCache()
    {
        await _cache.RemoveAsync(JogoCacheKeys.Todos);
        await _cache.RemoveAsync(JogoCacheKeys.Ativos);
    }
}
