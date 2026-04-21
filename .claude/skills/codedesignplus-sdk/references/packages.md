# CodeDesignPlus.Net.Sdk — Full package reference

For each package: **purpose → Options (section + properties) → key interfaces/abstractions → registration method**.

---

## CodeDesignPlus.Net.Core

Foundation layer. Provides domain-event infrastructure, microservice metadata, startup hooks.

**Options** — `CoreOptions`, section `"Core"`
- `Id` (Guid)
- `Business` (string, required)
- `AppName` (string, required)
- `TypeEntryPoint` (string)
- `Version` (string)
- `Description` (string)
- `Contact` (Contact: `Name`, `Email`)
- `PathBase` (string)
- `ApiDocumentationBaseUrl` (string)

**Interfaces / abstractions**
- `IDomainEventResolver` — resolves `IDomainEvent` types by `EventKey` attribute.
- `IEventContext` — ambient context for events produced during a request.
- `IStartup` — implement to run initialization after DI build (auto-discovered).
- `IDomainEvent`, `IAggregateRoot<TKey>`, `IEntity<TKey>`, `AggregateRoot<TKey>`, `Entity<TKey>` — DDD base types.

**Register:** `services.AddCore(configuration)` — must be called before any other `Add{X}`.

---

## CodeDesignPlus.Net.Exceptions

Custom exception hierarchy for domain/application/infrastructure errors.

**Options** — none.
**Classes:** `CodeDesignPlusException`, plus specialized per-layer exceptions.
**Register:** not applicable (pure library).

---

## CodeDesignPlus.Net.ValueObjects

DDD value objects, immutable, validated on construction.

**Classes:** `Money`, `Currency` (ISO 4217), `Location`.
**Register:** not applicable.

---

## CodeDesignPlus.Net.Serializers

JSON serialization with domain-event–aware contract resolver.

**Classes:** `EventContractResolver`, `JsonSerializer` helpers, `SerializersException`.
**Register:** consumed implicitly by PubSub / EventStore.

---

## CodeDesignPlus.Net.Criteria

Dynamic filter + sort DSL that compiles to `Expression<Func<T,bool>>`.

**Classes:** `Tokenizer`, `Parser`, `Evaluator`, `CriteriaExtensions`.
**Use:** `IQueryable<T>.Matching(criteria)` / `.OrderingBy(criteria)`.

---

## CodeDesignPlus.Net.Generator

Roslyn source generators — generate DTOs from domain models.
**Register:** added as `Analyzer` reference in `.csproj`.

---

## CodeDesignPlus.Net.EFCore

EF Core repository abstractions.

**Options** — `EFCoreOptions`, section `"EFCore"`
- `Enable` (bool, default `true`)
- `RegisterRepositories` (bool, default `true`) — auto-scans assembly for `IRepositoryBase<T>` implementations.

**Interfaces / abstractions**
- `IRepositoryBase<TEntity>` — `GetAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `MatchingAsync` (Criteria-aware).
- `IOperationBase<TEntity, TKey>` — `CreateAsync`, `UpdateAsync`, `DeleteAsync`.
- `OperationBase<TUser, TEntity, TKey>` — abstract base with audit field assignment (`CreatedBy`, `UpdatedBy`, `CreatedAt`, `IsActive`).

**Register:** `services.AddEFCore<TDbContext>(configuration)`.

---

## CodeDesignPlus.Net.Mongo

MongoDB repository abstractions.

**Options** — `MongoOptions`, section `"Mongo"`
- `Enable` (bool)
- `RegisterHealthCheck` (bool, default `true`)
- `ConnectionString` (string, required)
- `Database` (string, required)
- `SslProtocols` (`SslProtocols`, default `Tls12|Tls13`)
- `RegisterAutomaticRepositories` (bool, default `true`)

**Interfaces / abstractions**
- `IRepositoryBase` / `IRepositoryBase<TEntity, TKey>`
- `ICreateOperation<TEntity>`, `IUpdateOperation<TEntity>`, `IDeleteOperation<TEntity>`
- `IOperationBase<TEntity, TKey>`, `OperationBase<TUser, TEntity, TKey>` — parallel API to EFCore base.

**Register:** `services.AddMongo(configuration)`.

---

## CodeDesignPlus.Net.Cache

Abstract caching contract.

**Options** — none (lives in provider package).
**Interfaces:** `ICacheManager` — `GetAsync<T>`, `SetAsync<T>`, `ExistsAsync`, `RemoveAsync`, `ClearAsync`.
**Register:** provider-specific (see Redis.Cache).

---

## CodeDesignPlus.Net.Redis

Redis connection management (supports multiple named instances).

**Options** — `RedisOptions`, section `"Redis"`
- `RegisterHealthCheck` (bool, default `true`)
- `Instances` (`Dictionary<string, Instance>`): per-instance `ConnectionString`, `Password`, `Certificate`, `PasswordCertificate`.

**Interfaces**
- `IRedis` — exposes `IConnectionMultiplexer`, `IDatabase`, `ISubscriber`.
- `IRedisFactory` — `Create(string instanceName)` returns an `IRedis`.

**Register:** `services.AddRedis(configuration)`.

---

## CodeDesignPlus.Net.Redis.Cache

`ICacheManager` backed by Redis.

**Options** — `Redis.CacheOptions`, section `"Redis.Cache"` (TTL, key prefix, default instance).
**Register:** `services.AddCache(configuration)` (requires `AddRedis`).

---

## CodeDesignPlus.Net.PubSub

Publisher/subscriber contract with optional in-memory queue.

**Options** — `PubSubOptions`, section `"PubSub"`
- `UseQueue` (bool, default `true`) — buffer in memory before dispatch.
- `SecondsWaitQueue` (uint, 1–10, default `2`)
- `EnableDiagnostic` (bool, default `false`)
- `RegisterAutomaticHandlers` (bool, default `true`) — auto-register every `IEventHandler<T>` in loaded assemblies.

**Interfaces**
- `IPublisher` — `PublishAsync(IDomainEvent, CancellationToken)`.
- `ISubscriber` — `SubscribeAsync<TEvent, THandler>()`.
- `IEventHandler<TEvent>` — implement `HandleAsync(TEvent, CancellationToken)` in consumers.

**Register:** `services.AddPubSub(configuration)` + one transport below.

---

## CodeDesignPlus.Net.Kafka

Kafka transport for `IPublisher`/`ISubscriber`.

**Options** — `KafkaOptions : PubSubOptions`, section `"Kafka"`
- `Enable` (bool)
- `BootstrapServers` (string, required)
- `Acks` (`Acks`, default `All`)
- `BatchSize` (int)
- `LingerMs` (int)
- `CompressionType` (default `Snappy`)
- `SecurityProtocol` (default `Plaintext`)
- `MaxAttempts` (int, default `60`)
- Plus any SASL/SSL fields forwarded to `Confluent.Kafka`.

**Interfaces:** `IProducer`, `IConsumer` (wrappers around Confluent).
**Register:** `services.AddKafka(configuration)`.

---

## CodeDesignPlus.Net.RabbitMQ

RabbitMQ transport with dead-letter exchange support.

**Options** — `RabbitMQOptions`, section `"RabbitMQ"`
- `Enable`, `Host`, `Port` (default `5672`), `UserName`, `Password`, `VirtualHost`
- `RetryInterval`, `MaxRetry`, `QueueArguments`, exchange/queue naming conventions.

**Interfaces**
- `IRabbitConnection` — shared `IConnection`.
- `IChannelProvider` — pooled `IModel` channels for publishers/consumers.
- `IRabbitPubSubService` — publish + subscribe with DLX routing.

**Register:** `services.AddRabbitMQ(configuration)`.

---

## CodeDesignPlus.Net.Redis.PubSub

Redis pub/sub transport.

**Options** — `RedisPubSubOptions`, section `"Redis.PubSub"` (`Enable`, instance name).
**Interfaces:** `IRedisPubSubService`, `IEventHandler<T>` (from PubSub).
**Register:** `services.AddRedisPubSub(configuration)`.

---

## CodeDesignPlus.Net.Event.Sourcing

Core event-sourcing primitives (stream naming, snapshot cadence).

**Options** — `EventSourcingOptions`, section `"EventSourcing"`
- `MainName` (string, default `"aggregate"`)
- `SnapshotSuffix` (string, default `"snapshot"`)
- `FrequencySnapshot` (int, 1–500, default `20`)

**Interfaces:** `IEventSourcing` — `AppendEventAsync`, `LoadEventsAsync`, `SaveSnapshotAsync`, `LoadSnapshotAsync`.
**Register:** `services.AddEventSourcing(configuration)`.

---

## CodeDesignPlus.Net.EventStore

EventStoreDB implementation of `IEventSourcing`.

**Options** — `EventStoreOptions : EventSourcingOptions`, section `"EventStore"`
- `Servers` (`Dictionary<string, Server>`) — each `Server` has `ConnectionString`, `User`, `Password`.

**Interfaces**
- `IEventStore` : `IEventSourcing` — EventStore-specific query APIs.
- `IEventStoreConnection` — wraps EventStore client.
- `IEventStoreFactory` — creates connections per server key.

**Register:** `services.AddEventStore(configuration)`.

---

## CodeDesignPlus.Net.EventStore.PubSub

Bridges EventStore subscriptions into `IPublisher`/`ISubscriber`.

**Options** — `EventStorePubSubOptions`, section `"EventStore.PubSub"`.
**Register:** `services.AddEventStorePubSub(configuration)`.

---

## CodeDesignPlus.Net.Security

JWT auth + RBAC + tenant awareness.

**Options** — `SecurityOptions`, section `"Security"`
- `Authority` (string), `ClientId` (string)
- `IncludeErrorDetails`, `RequireHttpsMetadata`
- `ValidateAudience`, `ValidateIssuer`, `ValidateLifetime`
- `ValidIssuer` (string, required), `ValidIssuers` (List<string>)
- `ValidAudiences` (IEnumerable<string>, required)
- `Applications` (string[]) — allowed client apps
- `CertificatePath`, `CertificatePassword`
- `EnableTenantContext` (bool)
- `ValidateLicense` (bool)
- `ValidateRbac` (bool), `ServerRbac` (Uri), `RefreshRbacInterval` (ushort)

**Interfaces**
- `IUserContext` — scoped access to `IdUser`, `Name`, `Email`, `Tenant`, `Roles`, `Claims`.
- `ISecurityService` — token + RBAC validation utilities.

**Register:** `services.AddSecurity(configuration)` — also wires `AddAuthentication("Bearer")` + JWT bearer defaults.

---

## CodeDesignPlus.Net.Observability

OpenTelemetry metrics + traces.

**Options** — `ObservabilityOptions`, section `"Observability"`
- `Enable` (bool)
- `ServerOtel` (Uri) — OTLP endpoint.
- `Metrics` (`Metrics`: `Enable`, `AspNetCore`, `HttpClient`, `Runtime`, `Process`).
- `Trace` (`Trace`: `Enable`, `AspNetCore`, `HttpClient`, `Mongo`, `Redis`, `Kafka`, `RabbitMQ`, `EntityFramework`).

**Register:** `services.AddObservability(configuration)`.

---

## CodeDesignPlus.Net.Logger

Serilog + OTel log exporter.

**Options** — `LoggerOptions`, section `"Logger"`
- `Enable` (bool)
- `OTelEndpoint` (string, URL)
- `Level` (string, e.g. `Information`, `Debug`)

**Register:** `services.AddLogger(configuration)` (or `builder.Host.UseLogger()` for early-stage logging).

---

## CodeDesignPlus.Net.Vault

HashiCorp Vault integration — KV, Transit, and config provider.

**Options** — `VaultOptions`, section `"Vault"`
- `Enable` (bool)
- `Token` (string) — Token auth
- `Address` (Uri, required)
- `RoleId`, `SecretId` — AppRole auth
- `Solution` (string, required), `AppName` (string, required) — path prefix
- `KeyVault`, `Mongo`, `RabbitMQ`, `Kubernetes`, `Transit` — per-engine sub-options
- `TypeAuth` (computed: `Token` / `AppRole` / `Kubernetes` / `None`)

**Interfaces**
- `IVaultClient` — low-level Vault operations.
- `IVaultTransit` — encrypt/decrypt via Transit engine.
- `IVaultConfigurationProvider` — layer Vault KV into `IConfiguration`.

**Register:**
- Configuration: `builder.Configuration.AddVault(...)` during host build.
- Services: `services.AddVault(configuration)`.

---

## CodeDesignPlus.Net.File.Storage

Abstract file storage with multiple providers.

**Options** — `FileStorageOptions`, section `"FileStorage"`
- `AzureBlob` (`AzureBlobOptions`: `Enable`, `ConnectionString`, `ContainerName`, `UsePasswordLess`)
- `AzureFile` (`AzureFileOptions`: similar)
- `Local` (`LocalOptions`: `Enable`, `Path`)
- `UriDownload` (Uri, required)

**Interfaces:** `IFileStorageService` — `UploadAsync`, `DownloadAsync`, `DeleteAsync`, `ExistsAsync`; provider-specific services behind it.
**Register:** `services.AddFileStorage(configuration)`.

---

## CodeDesignPlus.Net.gRpc.Clients

Typed gRPC clients (country, currency, …) with caching.

**Options** — `GrpcClientsOptions`, section `"gRpcClients"` — per-client endpoint + credentials.
**Interfaces:** `IGrpcClientFactory`, `ICountryGrpc`, `ICurrencyGrpc`.
**Register:** `services.AddGrpcClients(configuration)` (adds memory cache for currency/country lookups).

---

## CodeDesignPlus.Net.Microservice.Commons

Cross-cutting patterns for REST + gRPC microservices: MediatR + FluentValidation pipeline behaviors, error response mapping, health-check wiring, gRPC interceptors.
**Register:** per-pattern extension methods (`AddRestCommons`, `AddGrpcCommons`, etc.).

---

## CodeDesignPlus.Net.xUnit

Integration-test fixtures wrapping Docker containers.

**Fixtures available:** SQL Server, MongoDB, Kafka, RabbitMQ, Vault, Redis (with/without PFX).
**Telemetry:** in-memory OTel exporters for traces/logs/metrics verification.

---

## CodeDesignPlus.Net.xUnit.Microservice

Test scaffolding for microservice integration tests.

**Classes**
- `ServerBase<TProgram>` / `Server<TProgram>` — WebApplicationFactory wrapper.
- `ServerCompose` — spins up Docker Compose dependencies.
- `InMemoryLoggerProvider` — asserts on log output.
- `GrpcChannel` helpers for typed gRPC test clients.
- `CommandAttribute<TAssemblyScan>` — xUnit data attribute that enumerates CQRS commands.
