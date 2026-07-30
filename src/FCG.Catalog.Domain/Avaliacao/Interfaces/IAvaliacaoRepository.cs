using FCG.Catalog.Domain.Avaliacao.Entities;

namespace FCG.Catalog.Domain.Avaliacao.Interfaces;

public interface IAvaliacaoRepository
{
    Task Adicionar(AvaliacaoEntity avaliacao);
    Task<IReadOnlyCollection<AvaliacaoEntity>> ObterPorJogoId(Guid jogoId);
}
