using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Jogo.Interfaces;

namespace FCG.Catalog.Application.Jogo.Services;

public class ListarJogosService
{
    private readonly IJogoRepository _jogoRepository;

    public ListarJogosService(IJogoRepository jogoRepository) => _jogoRepository = jogoRepository;

    public async Task<IReadOnlyCollection<JogoItemDto>> Execute()
    {
        var jogos = await _jogoRepository.ObterTodos();
        return jogos
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
