# FCG.Catalog

Microsserviço de **catálogo de jogos**, **biblioteca do usuário** e **pedidos de compra**. A API recebe a compra de forma assíncrona; o worker confirma o pedido após o processamento do pagamento.

## Projetos

| Projeto | Descrição |
|---|---|
| `FCG.Catalog.API` | API HTTP — catálogo, biblioteca e compras |
| `FCG.Catalog.Worker` | Consome `PaymentProcessedEvent` e atualiza o pedido |
| `FCG.Catalog.Application` | Casos de uso e consumidores de mensagens |
| `FCG.Catalog.Infrastructure` | EF Core (PostgreSQL) e RabbitMQ |
| `FCG.Catalog.Domain` | Entidades de jogos, biblioteca e pedidos |

## Imagens Docker

| Componente | Imagem |
|---|---|
| API | `gabrielnatan2001/fcg-api-catalog:latest` |
| Worker | `gabrielnatan2001/fcg-worker-catalog:latest` |

## Fluxo de compra

1. `POST /api/Biblioteca/{jogoId}/comprar` — cria pedido `Pending`, publica `OrderPlacedEvent`, retorna `202` com `{ orderId }`.
2. **FCG.Payments** consome o evento e publica `PaymentProcessedEvent`.
3. **FCG.Catalog.Worker** consome o pagamento: se aprovado, adiciona o jogo à biblioteca e marca o pedido como `Completed`; se rejeitado, marca como `Rejected`.

Na primeira execução, se não houver jogos, é criado o jogo de exemplo **Cyber Quest**.

## Variáveis de ambiente — API

| Variável (Docker/K8s) | appsettings | Obrigatória | Descrição | Exemplo |
|---|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | — | Sim | Ambiente de execução | `Production` |
| `ConnectionStrings__DefaultConnection` | `ConnectionStrings:DefaultConnection` | Sim | PostgreSQL (`fcg_catalog`) | `Host=postgres;Port=5432;Database=fcg_catalog;Username=postgres;Password=postgres` |
| `MessageBusConfigs__Host` | `MessageBusConfigs:Host` | Sim | URI do RabbitMQ | `amqp://admin:admin@rabbitmq:5672/` |
| `MessageBusConfigs__RetryCount` | `MessageBusConfigs:RetryCount` | Não | Tentativas de reconexão | `5` |
| `Publishers__OrderPlaced__Exchange` | `Publishers:OrderPlaced:Exchange` | Sim | Exchange do pedido | `fcg.order.placed` |
| `Publishers__OrderPlaced__RoutingKey` | `Publishers:OrderPlaced:RoutingKey` | Sim | Routing key do pedido | `payments.order-placed` |
| `Jwt__Key` | `Jwt:Key` | Sim | Chave JWT (mesma do Users) | *(secret)* |
| `Jwt__Issuer` | `Jwt:Issuer` | Sim | Emissor do token | `FCG.Users.API` |
| `Jwt__Audience` | `Jwt:Audience` | Sim | Audiência do token | `FCG.Client` |

## Variáveis de ambiente — Worker

| Variável (Docker/K8s) | appsettings | Obrigatória | Descrição | Exemplo |
|---|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | — | Sim | Ambiente de execução | `Production` |
| `ConnectionStrings__DefaultConnection` | `ConnectionStrings:DefaultConnection` | Sim | PostgreSQL (`fcg_catalog`) | `Host=postgres;Port=5432;Database=fcg_catalog;Username=postgres;Password=postgres` |
| `MessageBusConfigs__Host` | `MessageBusConfigs:Host` | Sim | URI do RabbitMQ | `amqp://admin:admin@rabbitmq:5672/` |
| `MessageBusConfigs__RetryCount` | `MessageBusConfigs:RetryCount` | Não | Tentativas de reconexão | `5` |
| `Workers__PaymentProcessed__Ativo` | `Workers:PaymentProcessed:Ativo` | Sim | Habilita o consumer | `true` |
| `Workers__PaymentProcessed__Exchange` | `Workers:PaymentProcessed:Exchange` | Sim | Exchange do pagamento | `fcg.payment.processed` |
| `Workers__PaymentProcessed__RoutingKey` | `Workers:PaymentProcessed:RoutingKey` | Sim | Routing key do pagamento | `catalog.payment-processed` |

## Executar localmente

```bash
dotnet ef database update --project src/FCG.Catalog.Infrastructure --startup-project src/FCG.Catalog.API
dotnet run --project src/FCG.Catalog.API
dotnet run --project src/FCG.Catalog.Worker
```

- Swagger: http://localhost:5002/swagger

Para subir a stack completa com Docker, use o [FCG.Infra](../FCG.Infra/README.md).

## Deploy

Manifests Kubernetes em `k8s/` (`api-*` e `worker-*`). Instruções completas no [README do FCG.Infra](../FCG.Infra/README.md).
