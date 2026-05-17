namespace CodeDesignPlus.Net.Hangfire.Test.Services;

/// <summary>
/// Pruebas unitarias para <see cref="JobService"/>.
/// </summary>
public class JobServiceTest
{
    private readonly Mock<IBackgroundJobClient> backgroundJobClientMock;
    private readonly Mock<IRecurringJobManager> recurringJobManagerMock;
    private readonly JobService jobService;

    /// <summary>
    /// Inicializa los mocks y la instancia del servicio bajo prueba.
    /// </summary>
    public JobServiceTest()
    {
        backgroundJobClientMock = new Mock<IBackgroundJobClient>();
        recurringJobManagerMock = new Mock<IRecurringJobManager>();
        jobService = new JobService(backgroundJobClientMock.Object, recurringJobManagerMock.Object);
    }

    /// <summary>
    /// Verifica que el constructor lanza <see cref="ArgumentNullException"/>
    /// cuando backgroundJobClient es nulo.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenBackgroundJobClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new JobService(null!, recurringJobManagerMock.Object));
    }

    /// <summary>
    /// Verifica que el constructor lanza <see cref="ArgumentNullException"/>
    /// cuando recurringJobManager es nulo.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRecurringJobManagerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new JobService(backgroundJobClientMock.Object, null!));
    }

    /// <summary>
    /// Verifica que <see cref="JobService.Enqueue{T}"/> delega correctamente
    /// la llamada al cliente de Hangfire (IBackgroundJobClient.Create).
    /// </summary>
    [Fact]
    public void Enqueue_DelegatesToBackgroundJobClient()
    {
        // Arrange
        backgroundJobClientMock
            .Setup(c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()))
            .Returns("job-id-1");

        // Act
        Expression<Action<FakeJob>> expr = job => job.DoWork();
        var id = jobService.Enqueue(expr);

        // Assert
        Assert.Equal("job-id-1", id);
        backgroundJobClientMock.Verify(
            c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que <see cref="JobService.Schedule{T}"/> delega correctamente
    /// la llamada al cliente de Hangfire con el delay especificado.
    /// </summary>
    [Fact]
    public void Schedule_DelegatesToBackgroundJobClient()
    {
        // Arrange
        backgroundJobClientMock
            .Setup(c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()))
            .Returns("job-id-2");

        // Act
        Expression<Action<FakeJob>> expr = job => job.DoWork();
        var id = jobService.Schedule(expr, TimeSpan.FromMinutes(5));

        // Assert
        Assert.Equal("job-id-2", id);
        backgroundJobClientMock.Verify(
            c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que <see cref="JobService.AddOrUpdateRecurring{T}"/> invoca
    /// al gestor de jobs recurrentes con UTC cuando no se especifica timezone.
    /// </summary>
    [Fact]
    public void AddOrUpdateRecurring_UsesUtc_WhenTimezoneIsNull()
    {
        // Act
        Expression<Action<FakeJob>> expr = job => job.DoWork();
        jobService.AddOrUpdateRecurring("fake-job", expr, Cron.Daily());

        // Assert
        recurringJobManagerMock.Verify(
            m => m.AddOrUpdate(
                "fake-job",
                It.IsAny<Job>(),
                Cron.Daily(),
                It.Is<RecurringJobOptions>(o => o.TimeZone == TimeZoneInfo.Utc)),
            Times.Once);
    }

    /// <summary>
    /// Verifica que <see cref="JobService.AddOrUpdateRecurring{T}"/> respeta
    /// el timezone cuando se especifica explícitamente.
    /// </summary>
    [Fact]
    public void AddOrUpdateRecurring_UsesSpecifiedTimezone()
    {
        // Act
        Expression<Action<FakeJob>> expr = job => job.DoWork();
        jobService.AddOrUpdateRecurring("fake-job-tz", expr, "0 6 * * *", "UTC");

        // Assert
        recurringJobManagerMock.Verify(
            m => m.AddOrUpdate(
                "fake-job-tz",
                It.IsAny<Job>(),
                "0 6 * * *",
                It.Is<RecurringJobOptions>(o => o.TimeZone.Id == "UTC")),
            Times.Once);
    }

    /// <summary>
    /// Verifica que <see cref="JobService.RemoveRecurring"/> invoca
    /// al gestor de jobs recurrentes con el ID correcto.
    /// </summary>
    [Fact]
    public void RemoveRecurring_DelegatesToRecurringJobManager()
    {
        // Act
        jobService.RemoveRecurring("my-recurring-job");

        // Assert
        recurringJobManagerMock.Verify(
            m => m.RemoveIfExists("my-recurring-job"),
            Times.Once);
    }

    /// <summary>
    /// Clase auxiliar para las pruebas que representa un job simple.
    /// </summary>
    public class FakeJob
    {
        /// <summary>Simula la ejecución de trabajo.</summary>
        public void DoWork() { }
    }
}
