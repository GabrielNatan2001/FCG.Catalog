using FCG.Catalog.Domain.Avaliacao.Entities;
using FCG.Catalog.Domain.Avaliacao.Interfaces;
using MongoDB.Driver;

namespace FCG.Catalog.Infrastructure.Data.Mongo;

public class AvaliacaoRepository : IAvaliacaoRepository
{
    private readonly IMongoCollection<AvaliacaoDocument> _collection;

    public AvaliacaoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<AvaliacaoDocument>("avaliacoes");
    }

    public async Task Adicionar(AvaliacaoEntity avaliacao)
    {
        var document = new AvaliacaoDocument
        {
            Id = avaliacao.Id,
            JogoId = avaliacao.JogoId,
            UserId = avaliacao.UserId,
            Nota = avaliacao.Nota,
            Comentario = avaliacao.Comentario,
            Data = avaliacao.Data
        };

        await _collection.InsertOneAsync(document);
    }

    public async Task<IReadOnlyCollection<AvaliacaoEntity>> ObterPorJogoId(Guid jogoId)
    {
        var documents = await _collection
            .Find(x => x.JogoId == jogoId)
            .SortByDescending(x => x.Data)
            .ToListAsync();

        return documents
            .Select(d => AvaliacaoEntity.Restaurar(d.Id, d.JogoId, d.UserId, d.Nota, d.Comentario, d.Data))
            .ToList();
    }
}
