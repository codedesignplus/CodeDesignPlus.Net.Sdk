# CodeDesignPlus.Net.Sdk — Usage patterns

Recipes that follow the SDK conventions. Copy the shape, adapt the types.

---

## 1. Service registration order (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddVault();                  // optional: layer Vault into config
builder.Host.UseLogger();                          // Serilog + OTel at host level

builder.Services
    .AddCore(builder.Configuration)                // MUST be first
    .AddSecurity(builder.Configuration)
    .AddObservability(builder.Configuration)
    .AddMongo(builder.Configuration)               // or AddEFCore<AppDbContext>
    .AddCache(builder.Configuration)               // Redis cache
    .AddPubSub(builder.Configuration)
    .AddRabbitMQ(builder.Configuration);           // transport

var app = builder.Build();
await app.RunAsync();
```

## 2. Domain aggregate + event

```csharp
public sealed class Order : AggregateRoot<Guid>
{
    private Order() { }
    public Order(Guid id, string customer) : base(id)
    {
        AddEvent(new OrderCreatedDomainEvent(id, customer));
    }
}

[EventKey<Order>(1, "OrderCreated")]
public sealed record OrderCreatedDomainEvent(Guid OrderId, string Customer) : DomainEvent(OrderId);
```

## 3. Event handler (PubSub)

```csharp
public sealed class OrderCreatedHandler(ILogger<OrderCreatedHandler> log)
    : IEventHandler<OrderCreatedDomainEvent>
{
    public Task HandleAsync(OrderCreatedDomainEvent @event, CancellationToken ct)
    {
        log.LogInformation("Order created {OrderId}", @event.OrderId);
        return Task.CompletedTask;
    }
}
// Auto-registered when PubSub:RegisterAutomaticHandlers = true.
```

## 4. Repository consumer (Mongo / EFCore — same shape)

```csharp
public sealed class OrderService(IRepositoryBase<Order, Guid> repo, IUserContext user)
{
    public Task CreateAsync(Order order, CancellationToken ct)
        => repo.AddAsync(order, user.IdUser, ct);
}
```

## 5. Custom `OperationBase` subclass

Use when you need auditing beyond what the default provides.

```csharp
public sealed class OrderOperations(IMongoContext ctx)
    : OperationBase<Guid, Order, Guid>(ctx) { }
```

## 6. Startup hook

```csharp
public sealed class SeedStartup(IServiceProvider sp) : IStartup
{
    public int Order => 10;
    public Task InitializeAsync(CancellationToken ct) { /* … */ return Task.CompletedTask; }
}
// Runs automatically after host build thanks to AddCore.
```

## 7. Options validation

Every `Options` class uses DataAnnotations. `Add{X}` calls `ValidateDataAnnotations().ValidateOnStart()`, so a bad `appsettings.json` fails the host boot — do not catch and ignore.

## 8. Integration test skeleton

```csharp
public class OrderApiTests(Server<Program> server) : IClassFixture<Server<Program>>
{
    [Fact]
    public async Task Create_returns_201()
    {
        using var client = server.CreateClient();
        var response = await client.PostAsJsonAsync("/orders", new { customer = "acme" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

For tests needing real infra, use `ServerCompose` (spins `docker-compose.yml` up/down) and fixtures from `CodeDesignPlus.Net.xUnit` (`MongoContainer`, `KafkaContainer`, …).

## 9. Criteria usage

```csharp
var criteria = new Criteria { Filters = "status=eq:active,amount=gt:100", OrderBy = "amount", OrderType = OrderTypes.Descending };
var result = await repo.MatchingAsync(criteria, ct);
```

## 10. Security — reading the current user

```csharp
public sealed class Handler(IUserContext user)
{
    public Task Handle(...) { var tenant = user.Tenant; var userId = user.IdUser; ... }
}
```

`IUserContext` is scoped; never cache across requests.
