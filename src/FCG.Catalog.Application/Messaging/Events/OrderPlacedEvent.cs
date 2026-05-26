namespace FCG.Catalog.Application.Messaging.Events;

public record OrderPlacedEvent(Guid OrderId, Guid UserId, Guid GameId, decimal Price, DateTime PlacedAtUtc);
