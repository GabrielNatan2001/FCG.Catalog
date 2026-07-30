using FCG.Catalog.Application.Biblioteca.Services;
using FCG.Catalog.Application.Jogo.Dtos;
using FCG.Catalog.Application.Jogo.Services;
using FCG.Catalog.Application.Messaging;
using FCG.Catalog.Application.Messaging.Events;
using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Biblioteca.Interfaces;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Domain.Pedidos.Entities;
using FCG.Catalog.Domain.Pedidos.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Catalog.Application.Tests;

internal static class TestCache
{
    public static IDistributedCache Create()
    {
        var cache = new Mock<IDistributedCache>();
        cache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        cache.Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        cache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return cache.Object;
    }

    public static IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cache:JogosTtlSeconds"] = "60"
            })
            .Build();
}

public class CriarJogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepository = new();

    [Fact]
    public async Task Execute_ComDadosValidos_DevePersistirJogo()
    {
        _jogoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);
        var service = new CriarJogoService(_jogoRepository.Object, TestCache.Create());
        var request = new CriarJogoDto.Request
        {
            Nome = "Cyber Quest",
            Descricao = "RPG",
            Preco = 59.99m,
            Categoria = "Aventura"
        };

        var id = await service.Execute(request);

        Assert.NotEqual(Guid.Empty, id);
        _jogoRepository.Verify(r => r.Adicionar(It.IsAny<JogoEntity>()), Times.Once);
        _jogoRepository.Verify(r => r.SalvarAlteracoes(), Times.Once);
    }
}

public class AtualizarJogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepository = new();

    [Fact]
    public async Task Execute_ComJogoInexistente_DeveLancarDomainException()
    {
        _jogoRepository.Setup(r => r.ObterPorId(It.IsAny<Guid>()))
            .ReturnsAsync((JogoEntity?)null);

        var service = new AtualizarJogoService(_jogoRepository.Object, TestCache.Create());
        var request = new AtualizarJogoDto.Request
        {
            Nome = "Novo",
            Descricao = "Desc",
            Preco = 10m,
            Categoria = "Ação"
        };

        var ex = await Assert.ThrowsAsync<DomainException>(() => service.Execute(Guid.NewGuid(), request));

        Assert.Equal("Jogo não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComJogoExistente_DeveAtualizar()
    {
        var jogo = JogoEntity.Criar("Original", "desc", 10m, "Ação");
        _jogoRepository.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
        _jogoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var service = new AtualizarJogoService(_jogoRepository.Object, TestCache.Create());
        var request = new AtualizarJogoDto.Request
        {
            Nome = "Atualizado",
            Descricao = "Nova desc",
            Preco = 20m,
            Categoria = "RPG"
        };

        await service.Execute(jogo.Id, request);

        Assert.Equal("Atualizado", jogo.Nome);
        _jogoRepository.Verify(r => r.Atualizar(jogo), Times.Once);
    }
}

public class AlterarStatusJogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepository = new();

    [Fact]
    public async Task Execute_ComJogoAtivo_DeveDesativar()
    {
        var jogo = JogoEntity.Criar("Jogo", "desc", 10m, "Ação");
        _jogoRepository.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
        _jogoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var service = new AlterarStatusJogoService(_jogoRepository.Object, TestCache.Create());

        await service.Execute(jogo.Id);

        Assert.Equal(EStatus.Inativo, jogo.Status);
    }

    [Fact]
    public async Task Execute_ComJogoInativo_DeveAtivar()
    {
        var jogo = JogoEntity.Criar("Jogo", "desc", 10m, "Ação");
        jogo.Desativar();
        _jogoRepository.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
        _jogoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var service = new AlterarStatusJogoService(_jogoRepository.Object, TestCache.Create());

        await service.Execute(jogo.Id);

        Assert.Equal(EStatus.Ativo, jogo.Status);
    }
}

public class ListarJogosServiceTests
{
    [Fact]
    public async Task Execute_DeveRetornarTodosOsJogos()
    {
        var jogos = new List<JogoEntity>
        {
            JogoEntity.Criar("Jogo 1", "desc", 10m, "Ação"),
            JogoEntity.Criar("Jogo 2", "desc", 20m, "RPG")
        };
        var repository = new Mock<IJogoRepository>();
        repository.Setup(r => r.ObterTodos()).ReturnsAsync(jogos);

        var service = new ListarJogosService(repository.Object, TestCache.Create(), TestCache.CreateConfiguration());

        var result = await service.Execute();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Nome == "Jogo 1");
        Assert.Contains(result, x => x.Nome == "Jogo 2");
    }
}

public class ListarJogosAtivosServiceTests
{
    [Fact]
    public async Task Execute_DeveRetornarApenasJogosAtivos()
    {
        var ativo = JogoEntity.Criar("Ativo", "desc", 10m, "Ação");
        var inativo = JogoEntity.Criar("Inativo", "desc", 20m, "RPG");
        inativo.Desativar();

        var repository = new Mock<IJogoRepository>();
        repository.Setup(r => r.ObterTodos()).ReturnsAsync(new[] { ativo, inativo });

        var service = new ListarJogosAtivosService(repository.Object, TestCache.Create(), TestCache.CreateConfiguration());

        var result = await service.Execute();

        Assert.Single(result);
        Assert.Equal("Ativo", result.Single().Nome);
    }
}

public class ComprarJogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepository = new();
    private readonly Mock<IPedidoRepository> _pedidoRepository = new();
    private readonly Mock<IBibliotecaRepository> _bibliotecaRepository = new();
    private readonly Mock<IMessageBus> _messageBus = new();
    private readonly OrderPlacedPublisherConfig _publisherConfig = new()
    {
        Exchange = "fcg.order.placed",
        RoutingKey = "payments.order-placed"
    };

    private ComprarJogoService CreateService() =>
        new(_jogoRepository.Object, _pedidoRepository.Object, _bibliotecaRepository.Object,
            _messageBus.Object, Options.Create(_publisherConfig));

    [Fact]
    public async Task Execute_ComJogoInexistente_DeveLancarDomainException()
    {
        _jogoRepository.Setup(r => r.ObterPorId(It.IsAny<Guid>()))
            .ReturnsAsync((JogoEntity?)null);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            CreateService().Execute(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("Jogo não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComJogoInativo_DeveLancarDomainException()
    {
        var jogo = JogoEntity.Criar("Jogo", "desc", 10m, "Ação");
        jogo.Desativar();
        _jogoRepository.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            CreateService().Execute(Guid.NewGuid(), jogo.Id));

        Assert.Equal("Este jogo não está disponível para compra.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComJogoJaNaBiblioteca_DeveLancarDomainException()
    {
        var usuarioId = Guid.NewGuid();
        var jogo = JogoEntity.Criar("Jogo", "desc", 10m, "Ação");
        var biblioteca = BibliotecaEntity.Criar(usuarioId);
        biblioteca.AdicionarJogo(jogo.Id);

        _jogoRepository.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
        _bibliotecaRepository.Setup(r => r.ObterPorUsuarioId(usuarioId)).ReturnsAsync(biblioteca);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            CreateService().Execute(usuarioId, jogo.Id));

        Assert.Equal("Este jogo já está na sua biblioteca.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComDadosValidos_DeveCriarPedidoEPublicarEvento()
    {
        var usuarioId = Guid.NewGuid();
        var jogo = JogoEntity.Criar("Jogo", "desc", 59.99m, "Ação");

        _jogoRepository.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
        _bibliotecaRepository.Setup(r => r.ObterPorUsuarioId(usuarioId))
            .ReturnsAsync((BibliotecaEntity?)null);
        _pedidoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var orderId = await CreateService().Execute(usuarioId, jogo.Id);

        Assert.NotEqual(Guid.Empty, orderId);
        _pedidoRepository.Verify(r => r.Adicionar(It.IsAny<PedidoEntity>()), Times.Once);
        _messageBus.Verify(m => m.Publish(
            _publisherConfig.Exchange,
            _publisherConfig.RoutingKey,
            It.IsAny<OrderPlacedEvent>()), Times.Once);
    }
}

public class ConfirmarPagamentoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepository = new();
    private readonly Mock<IJogoRepository> _jogoRepository = new();
    private readonly Mock<IBibliotecaRepository> _bibliotecaRepository = new();

    private ConfirmarPagamentoService CreateService() =>
        new(_pedidoRepository.Object, _jogoRepository.Object, _bibliotecaRepository.Object);

    [Fact]
    public async Task Execute_ComPedidoInexistente_DeveLancarDomainException()
    {
        _pedidoRepository.Setup(r => r.ObterPorId(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoEntity?)null);

        var payment = new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Approved", DateTime.UtcNow);

        var ex = await Assert.ThrowsAsync<DomainException>(() => CreateService().Execute(payment));

        Assert.Equal("Pedido não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComPedidoNaoPending_DeveRetornarSemAlterar()
    {
        var pedido = PedidoEntity.Criar(Guid.NewGuid(), Guid.NewGuid(), 10m);
        pedido.Completar();
        _pedidoRepository.Setup(r => r.ObterPorId(pedido.Id)).ReturnsAsync(pedido);

        var payment = new PaymentProcessedEvent(pedido.Id, pedido.UserId, pedido.GameId, "Approved", DateTime.UtcNow);

        await CreateService().Execute(payment);

        _bibliotecaRepository.Verify(r => r.Adicionar(It.IsAny<BibliotecaEntity>()), Times.Never);
        _pedidoRepository.Verify(r => r.SalvarAlteracoes(), Times.Never);
    }

    [Fact]
    public async Task Execute_ComPagamentoAprovadoESemBiblioteca_DeveCriarBibliotecaECompletarPedido()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var pedido = PedidoEntity.Criar(userId, gameId, 59.99m);
        var jogo = JogoEntity.Criar("Jogo", "desc", 59.99m, "Ação");

        _pedidoRepository.Setup(r => r.ObterPorId(pedido.Id)).ReturnsAsync(pedido);
        _jogoRepository.Setup(r => r.ObterPorId(gameId)).ReturnsAsync(jogo);
        _bibliotecaRepository.Setup(r => r.ObterPorUsuarioId(userId))
            .ReturnsAsync((BibliotecaEntity?)null);
        _pedidoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var payment = new PaymentProcessedEvent(pedido.Id, userId, gameId, "Approved", DateTime.UtcNow);

        await CreateService().Execute(payment);

        Assert.Equal(EPedidoStatus.Completed, pedido.Status);
        _bibliotecaRepository.Verify(r => r.Adicionar(It.Is<BibliotecaEntity>(b => b.PossuiJogo(gameId))), Times.Once);
    }

    [Fact]
    public async Task Execute_ComPagamentoRejeitado_DeveRejeitarPedido()
    {
        var pedido = PedidoEntity.Criar(Guid.NewGuid(), Guid.NewGuid(), 10m);
        _pedidoRepository.Setup(r => r.ObterPorId(pedido.Id)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var payment = new PaymentProcessedEvent(pedido.Id, pedido.UserId, pedido.GameId, "Rejected", DateTime.UtcNow);

        await CreateService().Execute(payment);

        Assert.Equal(EPedidoStatus.Rejected, pedido.Status);
    }
}

public class ListarBibliotecaServiceTests
{
    [Fact]
    public async Task Execute_DeveRetornarJogosDaBiblioteca()
    {
        var usuarioId = Guid.NewGuid();
        var jogos = new List<JogoEntity> { JogoEntity.Criar("Meu Jogo", "desc", 10m, "Ação") };
        var repository = new Mock<IBibliotecaRepository>();
        repository.Setup(r => r.ObterJogosDaBiblioteca(usuarioId)).ReturnsAsync(jogos);

        var service = new ListarBibliotecaService(repository.Object);

        var result = await service.Execute(usuarioId);

        Assert.Single(result);
        Assert.Equal("Meu Jogo", result.Single().Nome);
    }
}
