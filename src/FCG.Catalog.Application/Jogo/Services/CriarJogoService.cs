using FCG.Catalog.Application.Caching;
using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace FCG.Catalog.Application.Jogo.Services;

public class CriarJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IDistributedCache _cache;

    public CriarJogoService(IJogoRepository jogoRepository, IDistributedCache cache)
    {
        _jogoRepository = jogoRepository;
        _cache = cache;
    }

    public async Task<Guid> Execute(CriarJogoDto.Request request)
    {
        var jogo = JogoEntity.Criar(request.Nome, request.Descricao, request.Preco, request.Categoria);
        await _jogoRepository.Adicionar(jogo);
        await _jogoRepository.SalvarAlteracoes();
        await InvalidarCache();
        return jogo.Id;
    }

    private async Task InvalidarCache()
    {
        await _cache.RemoveAsync(JogoCacheKeys.Todos);
        await _cache.RemoveAsync(JogoCacheKeys.Ativos);
    }
}
