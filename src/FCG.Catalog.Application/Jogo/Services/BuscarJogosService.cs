using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Jogo.Services;

public class BuscarJogosService
{
    private readonly IJogoSearchRepository _searchRepository;

    public BuscarJogosService(IJogoSearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<IReadOnlyCollection<BuscarJogosDto.Response>> Execute(string? termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            throw new DomainException("Informe o termo de busca (q).");

        var hits = await _searchRepository.Buscar(termo.Trim());

        return hits
            .Select(h => new BuscarJogosDto.Response
            {
                Id = h.Id,
                Nome = h.Nome,
                Descricao = h.Descricao,
                Preco = h.Preco,
                Categoria = h.Categoria,
                Score = h.Score
            })
            .ToList();
    }
}
