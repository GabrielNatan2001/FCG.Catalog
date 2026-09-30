# FCG.Catalog

Microsserviço de **catálogo de jogos**, **biblioteca do usuário**, **pedidos de compra**, **avaliações** (MongoDB) e **busca avançada** (OpenSearch). Listagens de jogos usam **cache Redis**. A API recebe a compra de forma assíncrona; o worker confirma o pedido após o processamento do pagamento. Métricas Prometheus em `/metrics`.

## Projetos

| Projeto | Descrição |
|---|---|
| `FCG.Catalog.API` | API HTTP — catálogo, biblioteca, compras, avaliações e search |
| `FCG.Catalog.Worker` | Consome `PaymentProcessedEvent` e atualiza o pedido |
| `FCG.Catalog.Application` | Casos de uso e consumidores de mensagens |
| `FCG.Catalog.Infrastructure` | EF Core (PostgreSQL), MongoDB, Redis, OpenSearch e RabbitMQ |
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
| OpenSearch | Busca fuzzy + relevância (`GET api/Search?q=` / `GET /search?q=`) |

## Busca avançada (OpenSearch)

- Ao **criar**, **atualizar** ou **alterar status** de um jogo, o documento é indexado/atualizado no OpenSearch.
- No startup da API, o índice `jogos` é garantido e os jogos do Postgres são sincronizados.
- A busca usa **MultiMatch + Fuzziness.Auto** (tolerância a erros de digitação) e ordena por **`_score`** (relevância).
- Apenas jogos **ativos** entram no resultado.

Exemplos via Kong:

```http
GET http://localhost:8000/catalog/api/Search?q=cybr
Authorization: Bearer <token>

GET http://localhost:8000/catalog/search?q=odyss
Authorization: Bearer <token>
```

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
| `ConnectionStrings__OpenSearch` | Sim (API) | OpenSearch | `http://opensearch:9200` |
| `MongoDB__Database` | Não | Database Mongo | `fcg_catalog` |
| `OpenSearch__Index` | Não | Nome do índice | `jogos` |
| `Cache__JogosTtlSeconds` | Não | TTL cache jogos | `60` |
| `MessageBusConfigs__Host` | Sim | RabbitMQ | `amqp://admin:admin@rabbitmq:5672/` |
| `Jwt__Key` / `Jwt__Issuer` / `Jwt__Audience` | Sim | JWT (igual Users/Kong) | — |

## Executar localmente

```bash
dotnet run --project src/FCG.Catalog.API
dotnet run --project src/FCG.Catalog.Worker
```

Em demo Compose/K8s, acesse via **Kong**: `http://localhost:8000/catalog/...`. Guia: [FCG.Infra](../FCG.Infra/README.md).
