# FCG.Catalog

Microsserviço de **catálogo de jogos**, **biblioteca do usuário**, **pedidos de compra** e **avaliações** (MongoDB). Listagens de jogos usam **cache Redis**. A API recebe a compra de forma assíncrona; o worker confirma o pedido após o processamento do pagamento. Métricas Prometheus em `/metrics`.

## Projetos

| Projeto | Descrição |
|---|---|
| `FCG.Catalog.API` | API HTTP — catálogo, biblioteca, compras e avaliações |
| `FCG.Catalog.Worker` | Consome `PaymentProcessedEvent` e atualiza o pedido |
| `FCG.Catalog.Application` | Casos de uso e consumidores de mensagens |
| `FCG.Catalog.Infrastructure` | EF Core (PostgreSQL), MongoDB, Redis e RabbitMQ |
| `FCG.Catalog.Domain` | Entidades de jogos, biblioteca, pedidos e avaliações |

## Imagens Docker

| Componente | Imagem |
|---|---|
| API | `gabrielnatan2001/fcg-api-catalog:latest` |
| Worker | `gabrielnatan2001/fcg-worker-catalog:latest` |

## Persistência poliglota

| Store | Uso |
|---|---|
| PostgreSQL | Jogos, biblioteca, pedidos |
| MongoDB | Avaliações (`POST/GET api/Avaliacao`) |
| Redis | Cache de `GET api/Jogo` e `GET api/Jogo/ativos` (TTL configurável) |

## Fluxo de compra

1. `POST /api/Biblioteca/{jogoId}/comprar` — cria pedido `Pending`, publica `OrderPlacedEvent`, retorna `202` com `{ orderId }`.
2. **FCG.Payments** consome o evento e publica `PaymentProcessedEvent`.
3. **FCG.Catalog.Worker** consome o pagamento: se aprovado, adiciona o jogo à biblioteca e marca o pedido como `Completed`; se rejeitado, marca como `Rejected`.

Na primeira execução, se não houver jogos, é criado o jogo de exemplo **Cyber Quest**.

## Variáveis de ambiente — API

| Variável (Docker/K8s) | Obrigatória | Descrição | Exemplo |
|---|---|---|---|
| `ConnectionStrings__DefaultConnection` | Sim | PostgreSQL | `Host=postgres;Port=5432;Database=fcg_catalog;Username=postgres;Password=postgres` |
| `ConnectionStrings__MongoDB` | Sim (API) | MongoDB | `mongodb://mongodb:27017` |
| `ConnectionStrings__Redis` | Sim (API) | Redis | `redis:6379` |
| `MongoDB__Database` | Não | Database Mongo | `fcg_catalog` |
| `Cache__JogosTtlSeconds` | Não | TTL cache jogos | `60` |
| `MessageBusConfigs__Host` | Sim | RabbitMQ | `amqp://admin:admin@rabbitmq:5672/` |
| `Jwt__Key` / `Jwt__Issuer` / `Jwt__Audience` | Sim | JWT (igual Users/Kong) | — |

## Executar localmente

```bash
dotnet run --project src/FCG.Catalog.API
dotnet run --project src/FCG.Catalog.Worker
```

Em demo Compose/K8s, acesse via **Kong**: `http://localhost:8000/catalog/...`. Guia: [FCG.Infra](../FCG.Infra/README.md).
