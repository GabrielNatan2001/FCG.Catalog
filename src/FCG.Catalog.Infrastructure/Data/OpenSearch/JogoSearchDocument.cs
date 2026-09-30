namespace FCG.Catalog.Infrastructure.Data.OpenSearch;

public sealed class JogoSearchDocument
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Status { get; set; }
}
