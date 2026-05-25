using FCG.Catalog.Application.Biblioteca.Services;
using FCG.Catalog.Application.Messaging.Events;
using MassTransit;

namespace FCG.Catalog.Application.Messaging.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly ConfirmarPagamentoService _confirmarPagamentoService;

    public PaymentProcessedConsumer(ConfirmarPagamentoService confirmarPagamentoService) =>
        _confirmarPagamentoService = confirmarPagamentoService;

    public Task Consume(ConsumeContext<PaymentProcessedEvent> context) =>
        _confirmarPagamentoService.Execute(context.Message);
}
