using FCG.Catalog.Application.Messaging.Events;

namespace FCG.Catalog.Application.Messaging.Consumers;

public interface IPaymentProcessedMessage
{
    Task Consumir(PaymentProcessedEvent dados);
}
