---
name: codedesignplus-sdk
description: Reference for CodeDesignPlus.Net.Sdk — provides package catalog, configuration Options classes with their appsettings section names, registration extension methods (AddCore, AddMongo, AddKafka, …), and the main interfaces/abstractions exposed by each library. TRIGGER when the user works with any CodeDesignPlus.Net.* package (Core, EFCore, Mongo, Kafka, RabbitMQ, Redis, PubSub, Event.Sourcing, EventStore, Security, Vault, Observability, File.Storage, Logger, Criteria, Serializers, ValueObjects, gRpc.Clients, xUnit, Microservice.Commons, etc.), asks how to configure a CodeDesignPlus library, which options to set in appsettings.json, which interface to implement, or which Add{X} extension to call. SKIP for unrelated .NET libraries or when the user is not using the CodeDesignPlus SDK.
---

# CodeDesignPlus.Net.Sdk Reference

The SDK is a modular collection of .NET libraries for building microservices, following DDD, CQRS, Event Sourcing and clean-architecture patterns. Each package ships an `Abstractions` assembly (interfaces + `Options` record) and an implementation assembly that exposes a `ServiceCollectionExtensions.Add{X}` method reading from `IConfiguration`.

## How to use this skill

1. **Identify the package(s)** the user needs from the catalog below.
2. **Use the section name** from the package entry when reading/writing `appsettings.json`.
3. **Register with `services.Add{X}(configuration)`** — every package follows this convention. Always call `AddCore` first; other packages may depend on `CoreOptions`.
4. For deep details, consult the companion files:
   - `references/packages.md` — full per-package reference (all options, all interfaces).
   - `references/patterns.md` — recurring usage patterns (repositories, event handlers, pub/sub consumers, options validation).

## Configuration convention

Every options class lives in `CodeDesignPlus.Net.{Package}.Abstractions/Options/{Package}Options.cs` and exposes a public `const string Section`. The `Add{X}` method binds and validates it using `IOptions<T>` + DataAnnotations:

```csharp
// Program.cs / Startup.cs
builder.Services
    .AddCore(builder.Configuration)          // always first
    .AddSecurity(builder.Configuration)
    .AddObservability(builder.Configuration)
    .AddLogger(builder.Configuration)
    .AddMongo(builder.Configuration)         // or AddEFCore<TDbContext>
    .AddPubSub(builder.Configuration)        // + one transport below
    .AddRabbitMQ(builder.Configuration);     // or AddKafka / AddRedisPubSub
```

`appsettings.json` mirrors the section constants:

```json
{
  "Core":         { "AppName": "orders", "Business": "acme", "Version": "1.0.0", "Description": "...", "Contact": { ... } },
  "Security":     { "Authority": "...", "ValidIssuer": "...", "ValidAudiences": ["..."] },
  "Observability":{ "Enable": true, "ServerOtel": "http://otel:4317", "Metrics": { ... }, "Trace": { ... } },
  "Mongo":        { "ConnectionString": "mongodb://...", "Database": "orders" },
  "PubSub":       { "UseQueue": true, "RegisterAutomaticHandlers": true },
  "RabbitMQ":     { "Host": "rabbit", "UserName": "...", "Password": "..." }
}
```

## Package catalog (quick reference)

| Package | Section | Add method | Primary role |
|---|---|---|---|
| Core | `Core` | `AddCore` | Foundation: `CoreOptions`, `IDomainEventResolver`, `IEventContext`, `IStartup` |
| Exceptions | — | — | Custom exception hierarchy (no DI) |
| ValueObjects | — | — | DDD value objects (`Money`, `Currency`, `Location`) |
| Serializers | — | — | JSON + domain-event contract resolvers |
| Criteria | — | — | Dynamic query parser (Tokenizer → Parser → Evaluator) |
| Generator | — | — | Source generators (DTOs) |
| EFCore | `EFCore` | `AddEFCore<TDbContext>` | `IRepositoryBase`, `IOperationBase`, `OperationBase` |
| Mongo | `Mongo` | `AddMongo` | `IRepositoryBase`, `ICreateOperation`, `IUpdateOperation`, `IDeleteOperation` |
| Cache | — | — | `ICacheManager` abstraction |
| Redis | `Redis` | `AddRedis` | `IRedis`, `IRedisFactory`, multi-instance connections |
| Redis.Cache | `Redis.Cache` | `AddCache` | `ICacheManager` via Redis |
| PubSub | `PubSub` | `AddPubSub` | `IPublisher`, `ISubscriber`, `IEventHandler<T>` |
| Kafka | `Kafka` | `AddKafka` | Kafka transport for PubSub (`IProducer`, `IConsumer`) |
| RabbitMQ | `RabbitMQ` | `AddRabbitMQ` | `IRabbitConnection`, `IChannelProvider`, `IRabbitPubSubService` |
| Redis.PubSub | `Redis.PubSub` | `AddRedisPubSub` | `IRedisPubSubService` |
| Event.Sourcing | `EventSourcing` | `AddEventSourcing` | `IEventSourcing`, snapshot policy |
| EventStore | `EventStore` | `AddEventStore` | `IEventStore`, `IEventStoreConnection`, `IEventStoreFactory` |
| EventStore.PubSub | `EventStore.PubSub` | `AddEventStorePubSub` | Pub/Sub bridge over EventStore |
| Security | `Security` | `AddSecurity` | JWT auth, `IUserContext`, RBAC, tenant context |
| Observability | `Observability` | `AddObservability` | OpenTelemetry metrics + traces |
| Logger | `Logger` | `AddLogger` | Serilog + OTel exporter |
| Vault | `Vault` | `AddVault` + config provider | `IVaultClient`, `IVaultTransit`, Token/AppRole/K8s auth |
| File.Storage | `FileStorage` | `AddFileStorage` | `IFileStorageService` (Azure Blob/File/Local) |
| gRpc.Clients | `gRpcClients` | `AddGrpcClients` | `IGrpcClientFactory` + country/currency clients |
| Microservice.Commons | — | — | REST + gRPC host patterns, MediatR + FluentValidation wiring |
| xUnit | — | — | Docker-container fixtures (SQL/Mongo/Kafka/Vault/Redis) + in-memory OTel exporters |
| xUnit.Microservice | — | — | `Server<TProgram>`, `ServerCompose`, `InMemoryLoggerProvider`, gRPC test channels |

## Quick decision tree

- **Need persistence?** → `AddEFCore<TDbContext>` (relational) or `AddMongo` (document). Both expose `IRepositoryBase` + `OperationBase`.
- **Need messaging?** → `AddPubSub` + one transport: `AddRabbitMQ` (default), `AddKafka`, or `AddRedisPubSub`. Implement `IEventHandler<TEvent>` and the handler auto-registers when `RegisterAutomaticHandlers=true`.
- **Need event sourcing?** → `AddEventStore` (wraps `AddEventSourcing`). Inherit from `AggregateRoot` (in Core) and persist via `IEventStore`.
- **Need caching?** → `AddCache` (from Redis.Cache) → inject `ICacheManager`.
- **Need auth?** → `AddSecurity` → inject `IUserContext`. Validate tokens via the configured `Authority`/`ValidIssuer`.
- **Need secrets?** → `AddVault` at host-builder time to layer Vault secrets into `IConfiguration`.
- **Need observability?** → `AddObservability` + `AddLogger` for OTel metrics/traces/logs.
- **Need integration tests?** → reference `CodeDesignPlus.Net.xUnit` for container fixtures; `xUnit.Microservice` for `Server<TProgram>` + `InMemoryLoggerProvider`.

## Dependencies between packages

```
Core ── (required by everything)
├── Serializers
├── Exceptions
├── ValueObjects
├── Criteria
└── Abstractions contract types (IDomainEvent, IAggregateRoot, IEntity)

EFCore / Mongo ──► Core
PubSub ──► Core
  ├── Kafka ──► PubSub
  ├── RabbitMQ ──► PubSub
  └── Redis.PubSub ──► Redis, PubSub
Event.Sourcing ──► Core
  └── EventStore ──► Event.Sourcing
      └── EventStore.PubSub ──► EventStore, PubSub
Security ──► Core
Observability ──► Core
Logger ──► Core
Vault ──► Core
File.Storage ──► Core
gRpc.Clients ──► Core
Redis.Cache ──► Redis, Cache
Microservice.Commons ──► Core, Security, Observability, Logger
```

## When answering

- Always cite the **exact section constant** and **Add method**; never invent new section names.
- When the user asks "how do I configure X", show both the `appsettings.json` snippet and the `Program.cs` registration call.
- For custom logic, prefer implementing the existing interface (`IEventHandler<T>`, `IRepositoryBase`, `IUserContext`, `IStartup`) over creating parallel abstractions.
- Call out when a package **inherits options** (e.g. `KafkaOptions : PubSubOptions`, `EventStoreOptions : EventSourcingOptions`) — the parent section keys still apply.
- For testing, recommend fixtures from `CodeDesignPlus.Net.xUnit*` rather than hand-rolled Docker setups.
- For full property lists, section-by-section, read `references/packages.md`.
