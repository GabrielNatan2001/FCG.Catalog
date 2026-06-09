# FCG.Catalog

Microsserviço de catálogo de jogos, biblioteca do usuário e pedidos de compra (integração assíncrona com pagamentos via RabbitMQ).

## Projetos

| Projeto | Descrição |
|---|---|
| `FCG.Catalog.API` | API HTTP (catálogo, biblioteca, compras) |
| `FCG.Catalog.Worker` | Consome `PaymentProcessedEvent` e confirma pedidos |

## Configuração

| Chave | Descrição |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL (`fcg_catalog`, porta 5435) |
| `MessageBusConfigs:*` | RabbitMQ — API publica `OrderPlacedEvent`; worker consome `PaymentProcessedEvent` |
| `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` | Mesma chave do serviço Users (somente API) |

## Executar

```bash
dotnet ef database update --project src/FCG.Catalog.Infrastructure --startup-project src/FCG.Catalog.API
dotnet run --project src/FCG.Catalog.API
dotnet run --project src/FCG.Catalog.Worker
```

Swagger: http://localhost:5002/swagger

## Fluxo de compra

1. `POST /api/Biblioteca/{jogoId}/comprar` — cria pedido `Pending`, publica `OrderPlacedEvent`, retorna `202` com `{ orderId }`.
2. FCG.Payments consome o evento e publica `PaymentProcessedEvent`.
3. FCG.Catalog.Worker consome o pagamento: se aprovado, adiciona o jogo à biblioteca e marca o pedido como `Completed`; se rejeitado, marca como `Rejected`.

## Seed

Na primeira execução, se não houver jogos, é criado o jogo de exemplo **Cyber Quest**.
