using FCG.Catalog.Application.Biblioteca.Services;
using FCG.Catalog.Application.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Application.Messaging.Consumers;

public class PaymentProcessedConsumer : IPaymentProcessedMessage
{
    private readonly ConfirmarPagamentoService _confirmarPagamentoService;
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(
        ConfirmarPagamentoService confirmarPagamentoService,
        ILogger<PaymentProcessedConsumer> logger)
    {
        _confirmarPagamentoService = confirmarPagamentoService;
        _logger = logger;
    }

    public async Task Consumir(PaymentProcessedEvent dados)
    {
        _logger.LogInformation(
            "PaymentProcessedEvent recebido | OrderId: {OrderId} | Status: {Status}",
            dados.OrderId,
            dados.Status);

        await _confirmarPagamentoService.Execute(dados);
    }
}
