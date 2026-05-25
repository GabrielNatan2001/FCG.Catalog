using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Biblioteca.Interfaces;
using FCG.Catalog.Domain.Jogo.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.Data.Repositories;

public class BibliotecaRepository : IBibliotecaRepository
{
    private readonly AppDbContext _context;

    public BibliotecaRepository(AppDbContext context) => _context = context;

    public async Task Adicionar(BibliotecaEntity biblioteca) =>
        await _context.Bibliotecas.AddAsync(biblioteca);

    public void Atualizar(BibliotecaEntity biblioteca) => _context.Bibliotecas.Update(biblioteca);

    public void Remover(BibliotecaEntity biblioteca) => _context.Bibliotecas.Remove(biblioteca);

    public async Task<BibliotecaEntity?> ObterPorId(Guid id) =>
        await _context.Bibliotecas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<BibliotecaEntity?> ObterPorUsuarioId(Guid usuarioId) =>
        await _context.Bibliotecas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

    public async Task<IEnumerable<BibliotecaEntity>> ObterTodos() =>
        await _context.Bibliotecas.Include(x => x.Itens).ToListAsync();

    public async Task<IReadOnlyCollection<JogoEntity>> ObterJogosDaBiblioteca(Guid usuarioId)
    {
        var bibliotecaId = await _context.Bibliotecas
            .Where(b => b.UsuarioId == usuarioId)
            .Select(b => b.Id)
            .FirstOrDefaultAsync();

        if (bibliotecaId == Guid.Empty)
            return Array.Empty<JogoEntity>();

        return await _context.ItensBiblioteca
            .Where(x => x.BibliotecaId == bibliotecaId)
            .Select(x => x.Jogo)
            .ToListAsync();
    }

    public async Task<int> SalvarAlteracoes() => await _context.SaveChangesAsync();
}
