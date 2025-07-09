using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class RefreshRbacBackgroundServiceTest
{
    private readonly Mock<ILogger<RefreshRbacBackgroundService>> mockLogger;
    private readonly Mock<IRbac> mockRbacService;
    private readonly Mock<IOptions<SecurityOptions>> mockOptions;
    private readonly RefreshRbacBackgroundService service;

    public RefreshRbacBackgroundServiceTest()
    {
        mockLogger = new Mock<ILogger<RefreshRbacBackgroundService>>();
        mockRbacService = new Mock<IRbac>();
        mockOptions = new Mock<IOptions<SecurityOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new SecurityOptions { RefreshRbacInterval = 1 });

        service = new RefreshRbacBackgroundService(mockLogger.Object, mockRbacService.Object, mockOptions.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogInformation_WhenServiceIsStopping()
    {
        // Arrange
        using var stoppingTokenSource = new CancellationTokenSource();
        await stoppingTokenSource.CancelAsync();

        // Act
        await service.StartAsync(stoppingTokenSource.Token);

        // Assert
        mockLogger.VerifyLogging("RefreshRbacBackgroundService is stopping", LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogDebug_WhenRbacIsRefreshedSuccessfully()
    {
        // Arrange
        using var stoppingTokenSource = new CancellationTokenSource();
        stoppingTokenSource.CancelAfter(2000);

        // Act
        await service.StartAsync(stoppingTokenSource.Token);

        // Assert
        mockLogger.VerifyLogging("The RBAC was refreshed successfully", LogLevel.Debug, Times.AtLeastOnce());
        mockRbacService.Verify(r => r.LoadRbacAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        using var stoppingTokenSource = new CancellationTokenSource();
        mockRbacService.Setup(r => r.LoadRbacAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        await service.StartAsync(stoppingTokenSource.Token);

        // Assert
        mockLogger.VerifyLogging("An error occurred while refreshing the RBAC | Test exception", LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogInformation_WhenServiceIsRunning()
    {
        // Arrange
        using var stoppingTokenSource = new CancellationTokenSource();
        stoppingTokenSource.CancelAfter(TimeSpan.FromMinutes(3));

        // Act
        _ = service.StartAsync(stoppingTokenSource.Token);

        await Task.Delay(TimeSpan.FromSeconds(150));

        // Assert
        mockLogger.VerifyLogging("RefreshRbacBackgroundService is running", LogLevel.Information, Times.AtMostOnce());

    }
}
