using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Jogo.Services;

public class AlterarStatusJogoService
{
    private readonly IJogoRepository _jogoRepository;

    public AlterarStatusJogoService(IJogoRepository jogoRepository) => _jogoRepository = jogoRepository;

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
    }
}
