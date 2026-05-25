using FCG.Catalog.Domain.Base;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;

namespace FCG.Catalog.Domain.Jogo.Entities;

public class JogoEntity : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public string Categoria { get; private set; } = string.Empty;
    public EStatus Status { get; private set; }

    protected JogoEntity() { }

    private JogoEntity(string nome, string descricao, decimal preco, string categoria, EStatus status)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        Categoria = categoria;
        Status = status;
    }

    public static JogoEntity Criar(string nome, string descricao, decimal preco, string categoria)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do jogo é obrigatório.");

        if (preco < 0)
            throw new DomainException("Preço do jogo não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(categoria))
            throw new DomainException("Categoria do jogo é obrigatória.");

        return new JogoEntity(nome.Trim(), descricao?.Trim() ?? string.Empty, preco, categoria.Trim(), EStatus.Ativo);
    }

    public void Atualizar(string nome, string descricao, decimal preco, string categoria)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do jogo é obrigatório.");

        if (preco < 0)
            throw new DomainException("Preço do jogo não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(categoria))
            throw new DomainException("Categoria do jogo é obrigatória.");

        Nome = nome.Trim();
        Descricao = descricao?.Trim() ?? string.Empty;
        Preco = preco;
        Categoria = categoria.Trim();
    }

    public void Ativar() => Status = EStatus.Ativo;

    public void Desativar() => Status = EStatus.Inativo;
}
