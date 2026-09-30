using FCG.Catalog.Application.Caching;
using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace FCG.Catalog.Application.Jogo.Services;

public class AtualizarJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IJogoSearchRepository _searchRepository;
    private readonly IDistributedCache _cache;

    public AtualizarJogoService(
        IJogoRepository jogoRepository,
        IJogoSearchRepository searchRepository,
        IDistributedCache cache)
    {
        _jogoRepository = jogoRepository;
        _searchRepository = searchRepository;
        _cache = cache;
    }

    public async Task Execute(Guid id, AtualizarJogoDto.Request request)
    {
        var jogo = await _jogoRepository.ObterPorId(id);
        if (jogo is null)
            throw new DomainException("Jogo não encontrado.");

        jogo.Atualizar(request.Nome, request.Descricao, request.Preco, request.Categoria);
        _jogoRepository.Atualizar(jogo);
        await _jogoRepository.SalvarAlteracoes();
        await _searchRepository.Indexar(jogo);
        await InvalidarCache();
    }

    private async Task InvalidarCache()
    {
        await _cache.RemoveAsync(JogoCacheKeys.Todos);
        await _cache.RemoveAsync(JogoCacheKeys.Ativos);
    }
}
