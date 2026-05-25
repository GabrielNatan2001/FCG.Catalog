using FCG.Catalog.Domain.Base;
using FCG.Catalog.Domain.Exceptions;

namespace FCG.Catalog.Domain.Biblioteca.Entities;

public class BibliotecaEntity : BaseEntity
{
    public Guid UsuarioId { get; private set; }
    private readonly List<ItemBibliotecaEntity> _itens = new();
    public IReadOnlyCollection<ItemBibliotecaEntity> Itens => _itens.AsReadOnly();

    protected BibliotecaEntity() { }

    private BibliotecaEntity(Guid usuarioId) => UsuarioId = usuarioId;

    public static BibliotecaEntity Criar(Guid usuarioId) => new(usuarioId);

    public void AdicionarJogo(Guid jogoId)
    {
        if (PossuiJogo(jogoId))
            throw new DomainException("Este jogo já está na sua biblioteca.");

        _itens.Add(ItemBibliotecaEntity.Criar(jogoId));
        AtualizarDataAtualizacao();
    }

    public bool PossuiJogo(Guid jogoId) => _itens.Any(x => x.JogoId == jogoId);
}
