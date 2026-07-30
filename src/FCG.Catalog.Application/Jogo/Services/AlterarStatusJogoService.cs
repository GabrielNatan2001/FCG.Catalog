using FCG.Catalog.Application.Caching;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace FCG.Catalog.Application.Jogo.Services;

public class AlterarStatusJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IDistributedCache _cache;

    public AlterarStatusJogoService(IJogoRepository jogoRepository, IDistributedCache cache)
    {
        _jogoRepository = jogoRepository;
        _cache = cache;
    }

    public async Task Execute(Guid id)
    {
        var jogo = await _jogoRepository.ObterPorId(id);
        if (jogo is null)
            throw new DomainException("Jogo não encontrado.");

        if (jogo.Status == EStatus.Ativo)
            jogo.Desativar();
        else
            jogo.Ativar();

        _jogoRepository.Atualizar(jogo);
        await _jogoRepository.SalvarAlteracoes();
        await InvalidarCache();
    }

    private async Task InvalidarCache()
    {
        await _cache.RemoveAsync(JogoCacheKeys.Todos);
        await _cache.RemoveAsync(JogoCacheKeys.Ativos);
    }
}
