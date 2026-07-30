using FCG.Catalog.Domain.Exceptions;

namespace FCG.Catalog.Domain.Avaliacao.Entities;

public class AvaliacaoEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid JogoId { get; private set; }
    public Guid UserId { get; private set; }
    public int Nota { get; private set; }
    public string Comentario { get; private set; } = string.Empty;
    public DateTime Data { get; private set; } = DateTime.UtcNow;

    protected AvaliacaoEntity() { }

    private AvaliacaoEntity(Guid jogoId, Guid userId, int nota, string comentario)
    {
        JogoId = jogoId;
        UserId = userId;
        Nota = nota;
        Comentario = comentario;
    }

    public static AvaliacaoEntity Criar(Guid jogoId, Guid userId, int nota, string? comentario)
    {
        if (jogoId == Guid.Empty)
            throw new DomainException("JogoId é obrigatório.");

        if (userId == Guid.Empty)
            throw new DomainException("UserId é obrigatório.");

        if (nota is < 1 or > 5)
            throw new DomainException("Nota deve ser entre 1 e 5.");

        return new AvaliacaoEntity(jogoId, userId, nota, comentario?.Trim() ?? string.Empty);
    }

    public static AvaliacaoEntity Restaurar(Guid id, Guid jogoId, Guid userId, int nota, string comentario, DateTime data)
    {
        return new AvaliacaoEntity(jogoId, userId, nota, comentario)
        {
            Id = id,
            Data = data
        };
    }
}
