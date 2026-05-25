using MassTransit;

namespace FCG.Catalog.Infrastructure.Messaging;

internal static class RabbitMqEndpointExtensions
{
    public static void UsePreExistingQueue(this IRabbitMqReceiveEndpointConfigurator endpoint)
    {
        endpoint.ConfigureConsumeTopology = false;
    }
}
