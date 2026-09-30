using FCG.Catalog.Application.Messaging;
using FCG.Catalog.Domain.Avaliacao.Interfaces;
using FCG.Catalog.Domain.Biblioteca.Interfaces;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Domain.Pedidos.Interfaces;
using FCG.Catalog.Infrastructure.Data;
using FCG.Catalog.Infrastructure.Data.Mongo;
using FCG.Catalog.Infrastructure.Data.OpenSearch;
using FCG.Catalog.Infrastructure.Data.Repositories;
using FCG.Catalog.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenSearch.Client;

namespace FCG.Catalog.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var mongoConnection = configuration.GetConnectionString("MongoDB");
        if (!string.IsNullOrWhiteSpace(mongoConnection))
        {
            var mongoDatabaseName = configuration["MongoDB:Database"] ?? "fcg_catalog";
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnection));
            services.AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
            services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
        }

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "fcg-catalog:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        var openSearchUri = configuration.GetConnectionString("OpenSearch");
        if (!string.IsNullOrWhiteSpace(openSearchUri))
        {
            var settings = new ConnectionSettings(new Uri(openSearchUri))
                .DefaultIndex(configuration["OpenSearch:Index"] ?? "jogos")
                .DisableDirectStreaming();
            services.AddSingleton<IOpenSearchClient>(_ => new OpenSearchClient(settings));
            services.AddScoped<IJogoSearchRepository, JogoSearchRepository>();
        }
        else
        {
            services.AddSingleton<IJogoSearchRepository, NullJogoSearchRepository>();
        }

        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        services.Configure<OrderPlacedPublisherConfig>(
            configuration.GetSection("Publishers:OrderPlaced"));

        services.AddSingleton<IMessageBus, MessageBus>();
        services.AddRabbitMqTopologyInitializer(configuration);

        return services;
    }

    public static IServiceCollection AddHealthChecksInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection não configurado.");
        var rabbitHost = configuration["MessageBusConfigs:Host"]
            ?? throw new InvalidOperationException("MessageBusConfigs:Host não configurado.");

        var healthChecks = services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgres")
            .AddRabbitMQ(rabbitHost, name: "rabbitmq");

        var mongoConnection = configuration.GetConnectionString("MongoDB");
        if (!string.IsNullOrWhiteSpace(mongoConnection))
        {
            var mongoDatabaseName = configuration["MongoDB:Database"] ?? "fcg_catalog";
            healthChecks.AddMongoDb(
                clientFactory: sp => sp.GetRequiredService<IMongoClient>(),
                databaseNameFactory: _ => mongoDatabaseName,
                name: "mongodb");
        }

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
            healthChecks.AddRedis(redisConnection, name: "redis");

        var openSearchUri = configuration.GetConnectionString("OpenSearch");
        if (!string.IsNullOrWhiteSpace(openSearchUri))
        {
            healthChecks.AddAsyncCheck("opensearch", async () =>
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                var response = await http.GetAsync(openSearchUri.TrimEnd('/') + "/_cluster/health");
                return response.IsSuccessStatusCode
                    ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
                    : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy($"HTTP {(int)response.StatusCode}");
            });
        }

        return services;
    }
}
