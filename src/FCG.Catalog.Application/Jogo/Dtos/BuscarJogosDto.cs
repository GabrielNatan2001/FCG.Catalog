namespace FCG.Catalog.Application.Jogo.Dtos;

public static class BuscarJogosDto
{
    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}
