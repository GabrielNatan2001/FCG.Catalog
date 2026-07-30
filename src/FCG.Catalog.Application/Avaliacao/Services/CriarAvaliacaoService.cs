using FCG.Catalog.Application.Avaliacao.Dtos;
using FCG.Catalog.Domain.Avaliacao.Entities;
using FCG.Catalog.Domain.Avaliacao.Interfaces;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Avaliacao.Services;

public class CriarAvaliacaoService
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IJogoRepository _jogoRepository;

    public CriarAvaliacaoService(
        IAvaliacaoRepository avaliacaoRepository,
        IJogoRepository jogoRepository)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _jogoRepository = jogoRepository;
    }

    public async Task<Guid> Execute(Guid userId, CriarAvaliacaoDto.Request request)
    {
        var jogo = await _jogoRepository.ObterPorId(request.JogoId);
        if (jogo is null)
            throw new DomainException("Jogo não encontrado.");

        var avaliacao = AvaliacaoEntity.Criar(request.JogoId, userId, request.Nota, request.Comentario);
        await _avaliacaoRepository.Adicionar(avaliacao);
        return avaliacao.Id;
    }
}
