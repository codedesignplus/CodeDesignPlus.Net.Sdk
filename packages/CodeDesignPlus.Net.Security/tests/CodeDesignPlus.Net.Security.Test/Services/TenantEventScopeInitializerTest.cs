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

    private TenantEventScopeInitializer BuildInitializer() =>
        new(eventContextMock.Object, tenantMock.Object, Mock.Of<ILogger<TenantEventScopeInitializer>>());
}
