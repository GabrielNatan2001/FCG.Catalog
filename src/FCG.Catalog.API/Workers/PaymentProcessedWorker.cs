using FCG.Catalog.Application.Messaging;
using FCG.Catalog.Application.Messaging.Consumers;
using FCG.Catalog.Application.Messaging.Events;
using Microsoft.Extensions.Options;

namespace FCG.Catalog.API.Workers;

public class PaymentProcessedWorker : BackgroundService
{
    public readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentProcessedWorker> _logger;
    private readonly PaymentProcessedWorkerConfig _config;
    private const string NomeWorker = "WORKER-CATALOG-PAYMENT-PROCESSED";

    public PaymentProcessedWorker(
        IServiceProvider serviceProvider,
        ILogger<PaymentProcessedWorker> logger,
        IOptions<PaymentProcessedWorkerConfig> config)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_config.Ativo)
            {
                _logger.LogInformation("[WORKER][{Nome}] - Esta desativada.", NomeWorker);
                return;
            }

            _logger.LogInformation("[WORKER][{Nome}] Iniciado.", NomeWorker);

            using var scope = _serviceProvider.CreateScope();
            var _messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            await _messageBus.Subscribe<PaymentProcessedEvent>(
                _config.Exchange,
                _config.RoutingKey,
                ProcessaMensagem,
                stoppingToken);

            await Task.Delay(-1, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("[WORKER][{Nome}][EXCEPTION]: {Exception}", NomeWorker, ex.ToString());
        }
    }

    private async Task ProcessaMensagem(PaymentProcessedEvent dados)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var message = scope.ServiceProvider.GetRequiredService<IPaymentProcessedMessage>();
            await message.Consumir(dados);
        }
        catch (Exception ex)
        {
            _logger.LogError("[WORKER][{Nome}][EXCEPTION]: {Exception}", NomeWorker, ex.ToString());
        }
    }
}
