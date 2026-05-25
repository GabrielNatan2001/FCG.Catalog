using MassTransit;

namespace FCG.Catalog.Application.Messaging.Events;

[EntityName("fcg.order.placed")]
public record OrderPlacedEvent(Guid OrderId, Guid UserId, Guid GameId, decimal Price, DateTime PlacedAtUtc);
