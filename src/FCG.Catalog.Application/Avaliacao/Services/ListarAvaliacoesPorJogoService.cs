using FCG.Catalog.Application.Avaliacao.Dtos;
using FCG.Catalog.Domain.Avaliacao.Interfaces;

namespace FCG.Catalog.Application.Avaliacao.Services;

public class ListarAvaliacoesPorJogoService
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;

    public ListarAvaliacoesPorJogoService(IAvaliacaoRepository avaliacaoRepository) =>
        _avaliacaoRepository = avaliacaoRepository;

    public async Task<IReadOnlyCollection<AvaliacaoItemDto>> Execute(Guid jogoId)
    {
        var avaliacoes = await _avaliacaoRepository.ObterPorJogoId(jogoId);
        return avaliacoes
            .Select(x => new AvaliacaoItemDto
            {
                Id = x.Id,
                JogoId = x.JogoId,
                UserId = x.UserId,
                Nota = x.Nota,
                Comentario = x.Comentario,
                Data = x.Data
            })
            .ToList();
    }
}
