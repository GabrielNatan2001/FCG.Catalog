using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Infrastructure.Data.OpenSearch;

/// <summary>
/// Fallback quando OpenSearch não está configurado — indexação é no-op; busca falha com mensagem clara.
/// </summary>
public sealed class NullJogoSearchRepository : IJogoSearchRepository
{
    public Task GarantirIndice() => Task.CompletedTask;

    public Task Indexar(JogoEntity jogo) => Task.CompletedTask;

    public Task Remover(Guid id) => Task.CompletedTask;

    public Task<IReadOnlyCollection<JogoSearchHit>> Buscar(string termo) =>
        throw new DomainException("Busca avançada indisponível. Configure ConnectionStrings:OpenSearch.");
}
