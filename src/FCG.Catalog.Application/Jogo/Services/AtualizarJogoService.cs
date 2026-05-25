using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Jogo.Services;

public class AtualizarJogoService
{
    private readonly IJogoRepository _jogoRepository;

    public AtualizarJogoService(IJogoRepository jogoRepository) => _jogoRepository = jogoRepository;

    public async Task Execute(Guid id, AtualizarJogoDto.Request request)
    {
        var jogo = await _jogoRepository.ObterPorId(id);
        if (jogo is null)
            throw new DomainException("Jogo não encontrado.");

        jogo.Atualizar(request.Nome, request.Descricao, request.Preco, request.Categoria);
        _jogoRepository.Atualizar(jogo);
        await _jogoRepository.SalvarAlteracoes();
    }
}
