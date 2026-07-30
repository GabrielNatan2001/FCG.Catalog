namespace FCG.Catalog.Application.Avaliacao.Dtos;

public sealed class AvaliacaoItemDto
{
    public Guid Id { get; set; }
    public Guid JogoId { get; set; }
    public Guid UserId { get; set; }
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime Data { get; set; }
}
