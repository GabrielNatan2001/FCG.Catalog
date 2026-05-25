using FCG.Catalog.Application.Messaging.Events;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Domain.Pedidos.Entities;
using FCG.Catalog.Domain.Pedidos.Interfaces;
using MassTransit;

namespace FCG.Catalog.Application.Biblioteca.Services;

public class ComprarJogoService
{
    private readonly IJogoRepository _jogoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ComprarJogoService(
        IJogoRepository jogoRepository,
        IPedidoRepository pedidoRepository,
        IPublishEndpoint publishEndpoint)
    {
        _jogoRepository = jogoRepository;
        _pedidoRepository = pedidoRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Execute(Guid usuarioId, Guid jogoId)
    {
        var jogo = await _jogoRepository.ObterPorId(jogoId);
        if (jogo is null)
            throw new DomainException("Jogo não encontrado.");

        if (jogo.Status != EStatus.Ativo)
            throw new DomainException("Este jogo não está disponível para compra.");

        var pedido = PedidoEntity.Criar(usuarioId, jogoId, jogo.Preco);
        await _pedidoRepository.Adicionar(pedido);
        await _pedidoRepository.SalvarAlteracoes();

        var placedAt = DateTime.UtcNow;
        await _publishEndpoint.Publish(new OrderPlacedEvent(
            pedido.Id,
            usuarioId,
            jogoId,
            jogo.Preco,
            placedAt));

        return pedido.Id;
    }
}
