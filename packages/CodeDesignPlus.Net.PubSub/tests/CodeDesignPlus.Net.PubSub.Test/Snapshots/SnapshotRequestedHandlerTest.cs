using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Snapshots;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Snapshots;

/// <summary>
/// El dueño contesta una petición de foto que lo nombra con una foto por agregado, dirigida a quien pidió, y un cierre
/// con la cuenta (pendings/380).
/// </summary>
public class SnapshotRequestedHandlerTest
{
    private static readonly Guid TenantA = Guid.Parse("17daab5e-b090-48bd-969a-a7618eb2a726");
    private static readonly Guid TenantB = Guid.Parse("2b2b2b2b-2b2b-4b2b-8b2b-2b2b2b2b2b2b");

    private readonly List<IDomainEvent> published = [];
    private readonly Mock<IMessage> message = new();
    private readonly UserSnapshotSource source = new();

    public SnapshotRequestedHandlerTest()
    {
        this.message
            .Setup(x => x.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyList<IDomainEvent>, CancellationToken>((events, _) => this.published.AddRange(events))
            .Returns(Task.CompletedTask);

        this.message
            .Setup(x => x.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Callback<IDomainEvent, CancellationToken>((@event, _) => this.published.Add(@event))
            .Returns(Task.CompletedTask);

        for (var i = 0; i < 5; i++)
        {
            this.source.Users.Add(new UserEntity { Id = Guid.NewGuid(), Name = $"User {i}", Tenant = i < 3 ? TenantA : TenantB });
        }
    }

    [Fact]
    public async Task HandleAsync_RequestForThisOwner_PublishesEveryAggregateAcrossPagesStampedForTheRequester()
    {
        // Arrange
        var handler = this.Handler(appName: "xunit-owner", pageSize: 2);
        var request = SnapshotRequestedDomainEvent.Create("xunit-owner", nameof(UserEntity), "xunit-copy", null);

        // Act
        await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        var snapshots = this.published.OfType<UserSnapshotDomainEvent>().ToList();

        Assert.Equal((5, 5, 5), (snapshots.Select(x => x.AggregateId).Distinct().Count(), snapshots.Count(x => x.RequestId == request.RequestId), snapshots.Count(x => x.RequestedBy == "xunit-copy")));
    }

    [Fact]
    public async Task HandleAsync_RequestForThisOwner_ClosesWithTheCountAfterTheLastSnapshot()
    {
        // Arrange
        var handler = this.Handler(appName: "xunit-owner", pageSize: 2);
        var request = SnapshotRequestedDomainEvent.Create("xunit-owner", nameof(UserEntity), "xunit-copy", null);

        // Act
        await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        var completed = Assert.IsType<SnapshotCompletedDomainEvent>(this.published[^1]);

        Assert.Equal((request.RequestId, "xunit-copy", 5L), (completed.RequestId, completed.RequestedBy, completed.Count));
    }

    [Fact]
    public async Task HandleAsync_RequestForOneTenant_PublishesOnlyThatTenant()
    {
        // Arrange
        var handler = this.Handler(appName: "xunit-owner", pageSize: 100);
        var request = SnapshotRequestedDomainEvent.Create("xunit-owner", nameof(UserEntity), "xunit-copy", TenantB);

        // Act
        await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.All(this.published.OfType<UserSnapshotDomainEvent>(), x => Assert.Equal(TenantB, x.Tenant));
    }

    [Fact]
    public async Task HandleAsync_RequestForAnotherOwner_PublishesNothing()
    {
        // Arrange
        var handler = this.Handler(appName: "xunit-owner", pageSize: 2);
        var request = SnapshotRequestedDomainEvent.Create("ms-other", nameof(UserEntity), "xunit-copy", null);

        // Act
        await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.Empty(this.published);
    }

    [Fact]
    public async Task HandleAsync_AggregateWithoutSource_PublishesNothing()
    {
        // Arrange
        var handler = this.Handler(appName: "xunit-owner", pageSize: 2);
        var request = SnapshotRequestedDomainEvent.Create("xunit-owner", "InvoiceAggregate", "xunit-copy", null);

        // Act
        await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.Empty(this.published);
    }

    private SnapshotRequestedHandler Handler(string appName, uint pageSize)
    {
        var coreOptions = Microsoft.Extensions.Options.Options.Create(new CoreOptions
        {
            AppName = appName,
            Business = "CodeDesignPlus",
            Version = "v1",
            Description = "Snapshot tests",
            Contact = new() { Name = "CodeDesignPlus", Email = "wliscano@codedesignplus.com" }
        });

        var pubSubOptions = Microsoft.Extensions.Options.Options.Create(new PubSubOptions { SnapshotPageSize = pageSize });

        return new SnapshotRequestedHandler([this.source], this.message.Object, coreOptions, pubSubOptions, Microsoft.Extensions.Logging.Abstractions.NullLogger<SnapshotRequestedHandler>.Instance);
    }
}
