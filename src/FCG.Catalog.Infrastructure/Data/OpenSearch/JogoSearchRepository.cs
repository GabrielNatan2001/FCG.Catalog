using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenSearch.Client;

namespace FCG.Catalog.Infrastructure.Data.OpenSearch;

public class JogoSearchRepository : IJogoSearchRepository
{
    private readonly IOpenSearchClient _client;
    private readonly string _indexName;

    public JogoSearchRepository(IOpenSearchClient client, IConfiguration configuration)
    {
        _client = client;
        _indexName = configuration["OpenSearch:Index"] ?? "jogos";
    }

    public async Task GarantirIndice()
    {
        var exists = await _client.Indices.ExistsAsync(_indexName);
        if (exists.Exists)
            return;

        var create = await _client.Indices.CreateAsync(_indexName, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(0)
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("jogo_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("lowercase", "asciifolding")))))
            .Map<JogoSearchDocument>(m => m
                .Properties(p => p
                    .Keyword(k => k.Name(n => n.Id))
                    .Text(t => t.Name(n => n.Nome).Analyzer("jogo_analyzer"))
                    .Text(t => t.Name(n => n.Descricao).Analyzer("jogo_analyzer"))
                    .Text(t => t.Name(n => n.Categoria).Analyzer("jogo_analyzer"))
                    .Number(n => n.Name(x => x.Preco).Type(NumberType.Double))
                    .Number(n => n.Name(x => x.Status).Type(NumberType.Integer)))));

        if (!create.IsValid)
            throw new InvalidOperationException($"Falha ao criar índice OpenSearch '{_indexName}': {create.ServerError?.Error?.Reason ?? create.DebugInformation}");
    }

    public async Task Indexar(JogoEntity jogo)
    {
        var document = new JogoSearchDocument
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            Preco = jogo.Preco,
            Categoria = jogo.Categoria,
            Status = (int)jogo.Status
        };

        var response = await _client.IndexAsync(document, i => i
            .Index(_indexName)
            .Id(jogo.Id.ToString()));

        if (!response.IsValid)
            throw new InvalidOperationException($"Falha ao indexar jogo '{jogo.Id}': {response.ServerError?.Error?.Reason ?? response.DebugInformation}");

        await _client.Indices.RefreshAsync(_indexName);
    }

    public async Task Remover(Guid id)
    {
        var response = await _client.DeleteAsync<JogoSearchDocument>(id.ToString(), d => d
            .Index(_indexName));

        if (!response.IsValid && response.Result != Result.NotFound)
            throw new InvalidOperationException($"Falha ao remover jogo '{id}' do índice: {response.ServerError?.Error?.Reason ?? response.DebugInformation}");

        await _client.Indices.RefreshAsync(_indexName);
    }

    public async Task<IReadOnlyCollection<JogoSearchHit>> Buscar(string termo)
    {
        var response = await _client.SearchAsync<JogoSearchDocument>(s => s
            .Index(_indexName)
            .Size(50)
            .Query(q => q
                .Bool(b => b
                    .Must(mu => mu
                        .MultiMatch(mm => mm
                            .Query(termo)
                            .Fields(f => f
                                .Field(d => d.Nome, boost: 3)
                                .Field(d => d.Categoria, boost: 2)
                                .Field(d => d.Descricao))
                            .Fuzziness(Fuzziness.Auto)
                            .Type(TextQueryType.BestFields)
                            .Operator(Operator.Or)))
                    .Filter(f => f
                        .Term(t => t
                            .Field(d => d.Status)
                            .Value((int)EStatus.Ativo)))))
            .Sort(so => so
                .Descending(SortSpecialField.Score)));

        if (!response.IsValid)
            throw new InvalidOperationException($"Falha na busca OpenSearch: {response.ServerError?.Error?.Reason ?? response.DebugInformation}");

        return response.Hits
            .Select(hit => new JogoSearchHit(
                hit.Source.Id,
                hit.Source.Nome,
                hit.Source.Descricao,
                hit.Source.Preco,
                hit.Source.Categoria,
                hit.Score ?? 0))
            .ToList();
    }
}
