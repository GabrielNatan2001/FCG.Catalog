using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Common.Interfaces;
using FCG.Catalog.Domain.Jogo.Entities;

namespace FCG.Catalog.Domain.Biblioteca.Interfaces;

public interface IBibliotecaRepository : IRepository<BibliotecaEntity>
{
    Task<BibliotecaEntity?> ObterPorUsuarioId(Guid usuarioId);
    Task<IReadOnlyCollection<JogoEntity>> ObterJogosDaBiblioteca(Guid usuarioId);
}
