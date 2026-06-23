using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Pedidos.Entities;

namespace FCG.Catalog.Domain.Tests;

public class JogoEntityTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_DeveLancarDomainException(string nome)
    {
        var ex = Assert.Throws<DomainException>(() => JogoEntity.Criar(nome, "desc", 10m, "Ação"));

        Assert.Equal("Nome do jogo é obrigatório.", ex.Message);
    }

    [Fact]
    public void Criar_ComPrecoNegativo_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => JogoEntity.Criar("Jogo", "desc", -1m, "Ação"));

        Assert.Equal("Preço do jogo não pode ser negativo.", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComCategoriaVazia_DeveLancarDomainException(string categoria)
    {
        var ex = Assert.Throws<DomainException>(() => JogoEntity.Criar("Jogo", "desc", 10m, categoria));

        Assert.Equal("Categoria do jogo é obrigatória.", ex.Message);
    }

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarJogoAtivo()
    {
        var jogo = JogoEntity.Criar("  Cyber Quest  ", "  RPG  ", 59.99m, "  Aventura  ");

        Assert.Equal("Cyber Quest", jogo.Nome);
        Assert.Equal("RPG", jogo.Descricao);
        Assert.Equal(59.99m, jogo.Preco);
        Assert.Equal("Aventura", jogo.Categoria);
        Assert.Equal(EStatus.Ativo, jogo.Status);
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
    {
        var jogo = JogoEntity.Criar("Jogo", "desc", 10m, "Ação");

        jogo.Atualizar("Novo Nome", "Nova desc", 20m, "RPG");

        Assert.Equal("Novo Nome", jogo.Nome);
        Assert.Equal("Nova desc", jogo.Descricao);
        Assert.Equal(20m, jogo.Preco);
        Assert.Equal("RPG", jogo.Categoria);
    }

    [Fact]
    public void AtivarEDesativar_DeveAlternarStatus()
    {
        var jogo = JogoEntity.Criar("Jogo", "desc", 10m, "Ação");

        jogo.Desativar();
        Assert.Equal(EStatus.Inativo, jogo.Status);

        jogo.Ativar();
        Assert.Equal(EStatus.Ativo, jogo.Status);
    }
}

public class BibliotecaEntityTests
{
    [Fact]
    public void Criar_DeveDefinirUsuarioId()
    {
        var usuarioId = Guid.NewGuid();

        var biblioteca = BibliotecaEntity.Criar(usuarioId);

        Assert.Equal(usuarioId, biblioteca.UsuarioId);
        Assert.Empty(biblioteca.Itens);
    }

    [Fact]
    public void AdicionarJogo_ComJogoNovo_DeveAdicionarItem()
    {
        var biblioteca = BibliotecaEntity.Criar(Guid.NewGuid());
        var jogoId = Guid.NewGuid();

        biblioteca.AdicionarJogo(jogoId);

        Assert.True(biblioteca.PossuiJogo(jogoId));
        Assert.Single(biblioteca.Itens);
    }

    [Fact]
    public void AdicionarJogo_ComJogoDuplicado_DeveLancarDomainException()
    {
        var biblioteca = BibliotecaEntity.Criar(Guid.NewGuid());
        var jogoId = Guid.NewGuid();
        biblioteca.AdicionarJogo(jogoId);

        var ex = Assert.Throws<DomainException>(() => biblioteca.AdicionarJogo(jogoId));

        Assert.Equal("Este jogo já está na sua biblioteca.", ex.Message);
    }
}

public class PedidoEntityTests
{
    [Fact]
    public void Criar_ComPrecoNegativo_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => PedidoEntity.Criar(Guid.NewGuid(), Guid.NewGuid(), -1m));

        Assert.Equal("Preço do pedido não pode ser negativo.", ex.Message);
    }

    [Fact]
    public void Criar_ComDadosValidos_DeveIniciarComoPending()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var pedido = PedidoEntity.Criar(userId, gameId, 49.99m);

        Assert.Equal(userId, pedido.UserId);
        Assert.Equal(gameId, pedido.GameId);
        Assert.Equal(49.99m, pedido.Price);
        Assert.Equal(EPedidoStatus.Pending, pedido.Status);
    }

    [Fact]
    public void Completar_DeveAlterarStatusParaCompleted()
    {
        var pedido = PedidoEntity.Criar(Guid.NewGuid(), Guid.NewGuid(), 10m);

        pedido.Completar();

        Assert.Equal(EPedidoStatus.Completed, pedido.Status);
        Assert.NotNull(pedido.DtAtualizacao);
    }

    [Fact]
    public void Rejeitar_DeveAlterarStatusParaRejected()
    {
        var pedido = PedidoEntity.Criar(Guid.NewGuid(), Guid.NewGuid(), 10m);

        pedido.Rejeitar();

        Assert.Equal(EPedidoStatus.Rejected, pedido.Status);
        Assert.NotNull(pedido.DtAtualizacao);
    }
}
