using FCG.Catalog.Domain.Base;
using FCG.Catalog.Domain.Jogo.Entities;

namespace FCG.Catalog.Domain.Biblioteca.Entities;

public class ItemBibliotecaEntity : BaseEntity
{
    public Guid JogoId { get; private set; }
    public JogoEntity Jogo { get; private set; } = null!;
    public DateTime DataAquisicao { get; private set; }
    public Guid BibliotecaId { get; private set; }

    protected ItemBibliotecaEntity() { }

    private ItemBibliotecaEntity(Guid bibliotecaId, Guid jogoId)
    {
        BibliotecaId = bibliotecaId;
        JogoId = jogoId;
        DataAquisicao = DateTime.UtcNow;
    }

    public static ItemBibliotecaEntity Criar(Guid bibliotecaId, Guid jogoId) => new(bibliotecaId, jogoId);
}
