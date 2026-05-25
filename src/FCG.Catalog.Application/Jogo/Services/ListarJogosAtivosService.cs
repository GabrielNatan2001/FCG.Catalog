using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Jogo.Services;

public class ListarJogosAtivosService
{
    private readonly IJogoRepository _jogoRepository;

    public ListarJogosAtivosService(IJogoRepository jogoRepository) => _jogoRepository = jogoRepository;

    public async Task<IReadOnlyCollection<JogoItemDto>> Execute()
    {
        var jogos = await _jogoRepository.ObterTodos();
        return jogos
            .Where(x => x.Status == EStatus.Ativo)
            .Select(x => new JogoItemDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Descricao = x.Descricao,
                Preco = x.Preco,
                Categoria = x.Categoria,
                Status = x.Status
            })
            .ToList();
    }
}
