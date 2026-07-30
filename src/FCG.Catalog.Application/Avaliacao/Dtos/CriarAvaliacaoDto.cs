namespace FCG.Catalog.Application.Avaliacao.Dtos;

public sealed class CriarAvaliacaoDto
{
    public sealed class Request
    {
        public Guid JogoId { get; set; }
        public int Nota { get; set; }
        public string? Comentario { get; set; }
    }
}
