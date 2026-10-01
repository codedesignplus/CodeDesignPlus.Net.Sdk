using CodeDesignPlus.Net.Core.Abstractions.Contracts;
using CodeDesignPlus.Net.Core.Services;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.Core.Test.Contracts;

public class FilesReleasedDomainEventTest
{
    [Fact]
    public void GetKeyDomainEvent_AnyPublisher_ResolvesTheFileStorageTopic()
    {
        // The publisher's own AppName must not leak into the topic: every microservice writes to the one
        // ms-filestorage reads.
        var options = Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions);
        var service = new DomainEventResolverService(options);

        var key = service.GetKeyDomainEvent<FilesReleasedDomainEvent>();

        Assert.Equal("codedesignplus.ms-filestorage.v1.filestorageaggregate.filesreleaseddomainevent", key);
    }

    [Fact]
    public void Create_RepeatedFiles_KeepsEachOnce()
    {
        var file = Guid.NewGuid();
        var other = Guid.NewGuid();

        var @event = FilesReleasedDomainEvent.Create(Guid.NewGuid(), [file, other, file], Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal([file, other], @event.Files);
    }

    [Fact]
    public void Serialize_RoundTrip_KeepsFilesUserAndTenant()
    {
        var @event = FilesReleasedDomainEvent.Create(Guid.NewGuid(), [Guid.NewGuid(), Guid.NewGuid()], Guid.NewGuid(), Guid.NewGuid());

        var json = JsonSerializer.Serialize(@event);
        var copy = JsonSerializer.Deserialize<FilesReleasedDomainEvent>(json);

        Assert.NotNull(copy);
        Assert.Equal(@event.AggregateId, copy.AggregateId);
        Assert.Equal(@event.Files, copy.Files);
        Assert.Equal(@event.ReleasedBy, copy.ReleasedBy);
        Assert.Equal(@event.Tenant, copy.Tenant);
    }
}
