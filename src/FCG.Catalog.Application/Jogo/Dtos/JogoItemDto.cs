using FCG.Catalog.Domain.Common.Enums;

namespace FCG.Catalog.Application.Jogo.Dtos;

public sealed class JogoItemDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public EStatus Status { get; set; }
}
