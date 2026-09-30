using FCG.Catalog.Domain.Jogo.Entities;

namespace FCG.Catalog.Domain.Jogo.Interfaces;

public interface IJogoSearchRepository
{
    Task GarantirIndice();
    Task Indexar(JogoEntity jogo);
    Task Remover(Guid id);
    Task<IReadOnlyCollection<JogoSearchHit>> Buscar(string termo);
}

public sealed record JogoSearchHit(
    Guid Id,
    string Nome,
    string Descricao,
    decimal Preco,
    string Categoria,
    double Score);
