using FCG.Catalog.Application.Messaging;
using FCG.Catalog.Domain.Biblioteca.Interfaces;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Domain.Pedidos.Interfaces;
using FCG.Catalog.Infrastructure.Data;
using FCG.Catalog.Infrastructure.Data.Repositories;
using FCG.Catalog.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Catalog.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        services.Configure<OrderPlacedPublisherConfig>(
            configuration.GetSection("Publishers:OrderPlaced"));

        services.AddSingleton<IMessageBus, MessageBus>();

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

        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgres")
            .AddRabbitMQ(rabbitHost, name: "rabbitmq");

        return services;
    }
}
