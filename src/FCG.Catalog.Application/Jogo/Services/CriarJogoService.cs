using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Jogo.Services;

public class CriarJogoService
{
    private readonly IJogoRepository _jogoRepository;

    public CriarJogoService(IJogoRepository jogoRepository) => _jogoRepository = jogoRepository;

    public async Task<Guid> Execute(CriarJogoDto.Request request)
    {
        var jogo = JogoEntity.Criar(request.Nome, request.Descricao, request.Preco, request.Categoria);
        await _jogoRepository.Adicionar(jogo);
        await _jogoRepository.SalvarAlteracoes();
        return jogo.Id;
    }
}
