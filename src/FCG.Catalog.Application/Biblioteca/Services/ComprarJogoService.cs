using FCG.Catalog.Application.Messaging;
using FCG.Catalog.Application.Messaging.Events;
using FCG.Catalog.Domain.Biblioteca.Interfaces;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Domain.Pedidos.Entities;
using FCG.Catalog.Domain.Pedidos.Interfaces;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.Application.Biblioteca.Services;

public class ComprarJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IBibliotecaRepository _bibliotecaRepository;
    private readonly IMessageBus _messageBus;
    private readonly OrderPlacedPublisherConfig _publisherConfig;

    public ComprarJogoService(
        IJogoRepository jogoRepository,
        IPedidoRepository pedidoRepository,
        IBibliotecaRepository bibliotecaRepository,
        IMessageBus messageBus,
        IOptions<OrderPlacedPublisherConfig> publisherConfig)
    {
        _jogoRepository = jogoRepository;
        _pedidoRepository = pedidoRepository;
        _bibliotecaRepository = bibliotecaRepository;
        _messageBus = messageBus;
        _publisherConfig = publisherConfig.Value;
    }

    public async Task<Guid> Execute(Guid usuarioId, Guid jogoId)
    {
        var jogo = await _jogoRepository.ObterPorId(jogoId);
        if (jogo is null)
            throw new DomainException("Jogo não encontrado.");

        if (jogo.Status != EStatus.Ativo)
            throw new DomainException("Este jogo não está disponível para compra.");

        var biblioteca = await _bibliotecaRepository.ObterPorUsuarioId(usuarioId);
        if (biblioteca is not null && biblioteca.PossuiJogo(jogoId))
            throw new DomainException("Este jogo já está na sua biblioteca.");

        var pedido = PedidoEntity.Criar(usuarioId, jogoId, jogo.Preco);
        await _pedidoRepository.Adicionar(pedido);
        await _pedidoRepository.SalvarAlteracoes();

        var placedAt = DateTime.UtcNow;
        _messageBus.Publish(
            _publisherConfig.Exchange,
            _publisherConfig.RoutingKey,
            new OrderPlacedEvent(
                pedido.Id,
                usuarioId,
                jogoId,
                jogo.Preco,
                placedAt));

        return pedido.Id;
    }
}
