using FCG.Catalog.Application.Biblioteca.Services;
using FCG.Catalog.Application.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FCG.Catalog.Infrastructure.Messaging;

internal static class RabbitMqBusConfiguration
{
    public static void ConfigureHost(IRabbitMqBusFactoryConfigurator cfg, IConfiguration configuration)
    {
        var rabbit = configuration.GetSection("RabbitMq");
        cfg.Host(
            rabbit["Host"] ?? "localhost",
            ushort.Parse(rabbit["Port"] ?? "5672"),
            rabbit["VirtualHost"] ?? "/",
            h =>
            {
                h.Username(rabbit["Username"] ?? "guest");
                h.Password(rabbit["Password"] ?? "guest");
            });

        cfg.DeployPublishTopology = false;
    }

    public static void ConfigureConsumerAndPublish(
        IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        IConfiguration configuration)
    {
        var queues = configuration.GetSection("RabbitMq:Queues");
        var paymentProcessedQueue = queues["CatalogPaymentProcessed"] ?? "catalog.payment-processed";

        cfg.Publish<OrderPlacedEvent>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint(paymentProcessedQueue, e =>
        {
            e.UsePreExistingQueue();

            e.Handler<PaymentProcessedEvent>(async consumeContext =>
            {
                var service = consumeContext.GetServiceOrCreateInstance<ConfirmarPagamentoService>();
                var logger = consumeContext.GetServiceOrCreateInstance<ILogger<ConfirmarPagamentoService>>();

                logger.LogInformation(
                    "PaymentProcessedEvent recebido | OrderId: {OrderId} | Status: {Status}",
                    consumeContext.Message.OrderId,
                    consumeContext.Message.Status);

                await service.Execute(consumeContext.Message);
            });
        });
    }
}
