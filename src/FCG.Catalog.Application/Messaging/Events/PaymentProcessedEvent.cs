namespace FCG.Catalog.Application.Messaging.Events;

public record PaymentProcessedEvent(Guid OrderId, Guid UserId, Guid GameId, string Status, DateTime ProcessedAtUtc);
