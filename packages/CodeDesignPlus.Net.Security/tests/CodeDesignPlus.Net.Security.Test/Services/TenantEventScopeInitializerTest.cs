using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Logging;
using Moq;
using IDomainEvent = CodeDesignPlus.Net.Core.Abstractions.IDomainEvent;
using IEventContext = CodeDesignPlus.Net.Core.Abstractions.IEventContext;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class TenantEventScopeInitializerTest
{
    private readonly Mock<IEventContext> eventContextMock = new();
    private readonly Mock<ITenant> tenantMock = new();

    [Fact]
    public async Task InitializeAsync_EventCarriesTenant_LoadsIt()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        eventContextMock.SetupGet(c => c.Tenant).Returns(tenantId);

        var initializer = BuildInitializer();

        // Act
        await initializer.InitializeAsync(Mock.Of<IDomainEvent>());

        // Assert
        tenantMock.Verify(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_EventWithoutTenant_DoesNothing()
    {
        // Arrange: pasa cuando el evento no implementa Core.ITenant, asi que EventContext queda vacio.
        eventContextMock.SetupGet(c => c.Tenant).Returns(Guid.Empty);

        var initializer = BuildInitializer();

        // Act
        await initializer.InitializeAsync(Mock.Of<IDomainEvent>());

        // Assert
        tenantMock.Verify(t => t.SetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InitializeAsync_TenantCannotBeResolved_Rethrows()
    {
        // Arrange: propagar es lo que impide el ACK y deja que el broker reintente.
        var tenantId = Guid.NewGuid();
        eventContextMock.SetupGet(c => c.Tenant).Returns(tenantId);
        tenantMock.Setup(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>())).ThrowsAsync(new SecurityException("boom"));

        var initializer = BuildInitializer();

        // Act & Assert
        await Assert.ThrowsAsync<SecurityException>(() => initializer.InitializeAsync(Mock.Of<IDomainEvent>()));
    }

    /// <summary>
    /// La foto de una copropiedad dada de baja no lanza: la descarta el consumidor de fotos en vez de mandarla a la cola de
    /// descartados (pendings/467).
    /// </summary>
    [Fact]
    public async Task InitializeAsync_SnapshotOfAMissingTenant_DoesNotThrow()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        eventContextMock.SetupGet(c => c.Tenant).Returns(tenantId);
        tenantMock.Setup(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>())).ThrowsAsync(new CodeDesignPlusException(Layer.None, Errors.TenantNotFound));

        var initializer = BuildInitializer();

        // Act
        var exception = await Record.ExceptionAsync(() => initializer.InitializeAsync(new GoneSnapshotDomainEvent(Guid.NewGuid(), tenantId)));

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// A regular event of a tenant that is not found yet is retried, not dead-lettered as a business error: when a property
    /// is born, an event can reach a consumer before ms-tenants publishes the tenant (Kappali pendings/720).
    /// </summary>
    [Fact]
    public async Task InitializeAsync_RegularEventOfAMissingTenant_ThrowsARetryableError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        eventContextMock.SetupGet(c => c.Tenant).Returns(tenantId);
        tenantMock.Setup(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>())).ThrowsAsync(new CodeDesignPlusException(Layer.None, Errors.TenantNotFound));

        var initializer = BuildInitializer();

        // Act
        var exception = await Assert.ThrowsAsync<SecurityException>(() => initializer.InitializeAsync(Mock.Of<IDomainEvent>()));

        // Assert: a SecurityException and not a CodeDesignPlusException, which the brokers treat as non-retryable; the cause
        // travels inside.
        Assert.IsType<CodeDesignPlusException>(exception.InnerException);
    }

    /// <summary>
    /// Any other business error while loading the tenant is still rethrown as it is.
    /// </summary>
    [Fact]
    public async Task InitializeAsync_OtherBusinessError_IsRethrownAsItIs()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        eventContextMock.SetupGet(c => c.Tenant).Returns(tenantId);
        tenantMock.Setup(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>())).ThrowsAsync(new CodeDesignPlusException(Layer.None, "999", "Other"));

        var initializer = BuildInitializer();

        // Act & Assert
        await Assert.ThrowsAsync<CodeDesignPlusException>(() => initializer.InitializeAsync(Mock.Of<IDomainEvent>()));
    }

    private sealed class GoneSnapshotDomainEvent(Guid aggregateId, Guid tenant) : SnapshotDomainEvent(aggregateId, tenant);

    private TenantEventScopeInitializer BuildInitializer() =>
        new(eventContextMock.Object, tenantMock.Object, Mock.Of<ILogger<TenantEventScopeInitializer>>());
}
