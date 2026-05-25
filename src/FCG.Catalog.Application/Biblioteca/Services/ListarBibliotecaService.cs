using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Domain.Biblioteca.Interfaces;

namespace FCG.Catalog.Application.Biblioteca.Services;

public class ListarBibliotecaService
{
    private readonly IBibliotecaRepository _bibliotecaRepository;

    public ListarBibliotecaService(IBibliotecaRepository bibliotecaRepository) =>
        _bibliotecaRepository = bibliotecaRepository;

    public async Task<IReadOnlyCollection<JogoItemDto>> Execute(Guid usuarioId)
    {
        var jogos = await _bibliotecaRepository.ObterJogosDaBiblioteca(usuarioId);
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
