using CodeDesignPlus.Net.PubSub.Diagnostics;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace CodeDesignPlus.Net.PubSub.Test.Diagnostics;

public class ActivitySourceServiceTest
{

    public ActivitySourceServiceTest()
    {
        var activityListener = new ActivityListener
        {
            ShouldListenTo = s => true,
            SampleUsingParentId = (ref ActivityCreationOptions<string> activityOptions) => ActivitySamplingResult.AllData,
            Sample = (ref ActivityCreationOptions<ActivityContext> activityOptions) => ActivitySamplingResult.AllData

        };

        ActivitySource.AddActivityListener(activityListener);
    }

    [Fact]
    public void AddActivity_ShouldReturnTrue_WhenActivityAddedSuccessfully()
    {
        // Arrange
        var service = new ActivitySourceService();
        var activity = new Activity("TestActivity");

        // Act
        var result = service.AddActivity(1, activity);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RemoveActivity_ShouldReturnTrue_WhenActivityRemovedSuccessfully()
    {
        // Arrange
        var service = new ActivitySourceService();
        var activity = new Activity("TestActivity");
        service.AddActivity(1, activity);

        // Act
        var result = service.RemoveActivity(1, out var removedActivity);

        // Assert
        Assert.True(result);
        Assert.Equal(activity, removedActivity);
    }

    [Fact]
    public void StartActivity_ShouldReturnNonNullActivity_WhenCalled()
    {
        // Arrange
        var service = new ActivitySourceService();

        // Act
        var activity = service.StartActivity("TestActivity", ActivityKind.Server);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("TestActivity", activity.OperationName);
        Assert.Equal(ActivityKind.Server, activity.Kind);
    }

    [Fact]
    public void StartActivity_ShouldReturnNonNullActivity_WhenPropagationContextIsNotNull()
    {
        // Arrange
        var service = new ActivitySourceService();
        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();
        var activityContext = new ActivityContext(traceId, spanId, ActivityTraceFlags.Recorded);

        // Act
        var activity = service.StartActivity("TestActivity", ActivityKind.Server, new PropagationContext(activityContext, new OpenTelemetry.Baggage()));

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("TestActivity", activity.OperationName);
        Assert.Equal(ActivityKind.Server, activity.Kind);
        Assert.Equal(spanId, activity.ParentSpanId);
        Assert.Contains(traceId.ToString(), activity.ParentId);
    }

    [Fact]
    public void TryGetActivity_ShouldReturnTrue_WhenActivityExists()
    {
        // Arrange
        var service = new ActivitySourceService();
        var activity = new Activity("TestActivity");
        service.AddActivity(1, activity);

        // Act
        var result = service.TryGetActivity(1, out var resultActivity);

        // Assert
        Assert.True(result);
        Assert.Equal(activity, resultActivity);
    }

    [Fact]
    public void Inject_ShouldInjectTraceContextIntoMetadata_WhenActivityIsNotNull()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var service = new ActivitySourceService();
        var domainEvent = new UserRegisteredEvent(idAggregate)
        {
            Name = "Test",
            User = "usrtest",
            Age = 25
        };

        var activity = new Activity("TestActivity");
        activity.Start();

        // Act
        var exception = Record.Exception(() => service.Inject(activity, domainEvent));

        activity.Stop();

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Inject_NullActivity_UsesCurrentActivityContext()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var activityService = new ActivitySourceService();
        var domainEvent = new UserRegisteredEvent(idAggregate)
        {
            Name = "Test",
            User = "usrtest",
            Age = 25
        };

        var activity = new Activity("TestActivity");
        activity.Start();
        Activity.Current = activity;

        // Act
        var exception = Record.Exception(() => activityService.Inject(null, domainEvent));

        // Assert
        Assert.Null(exception);

        // Cleanup
        activity.Stop();
        Activity.Current = null;
    }

    [Fact]
    public void Extract_ContextIsExtractedFromDomainEvent()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var activityService = new ActivitySourceService();
        var metadata = new Dictionary<string, object>
        {
            { "traceparent", "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01" }
        };
        var domainEvent = new UserRegisteredEvent(idAggregate, metadata: metadata)
        {
            Name = "Test",
            User = "usrtest",
            Age = 25
        };

        // Act
        Activity.Current = null;

        var exception = Record.Exception(() => activityService.Extract(domainEvent));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ExtractTraceContextFromBasicProperties_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var domainEvent = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "Test",
            User = "usrtest",
            Age = 25
        };
        domainEvent.Metadata.Add("traceparent", "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01");

        // Act
        var result = ActivitySourceService.ExtractTraceContextFromBasicProperties(domainEvent, "traceparent");

        // Assert
        Assert.Single(result);
        Assert.Equal("00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01", result.First());
    }

    [Fact]
    public void ExtractTraceContextFromBasicProperties_WithNonExistingKey_ReturnsEmpty()
    {
        // Arrange 
        var domainEvent = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "Test",
            User = "usrtest",
            Age = 25
        };

        // Act
        var result = ActivitySourceService.ExtractTraceContextFromBasicProperties(domainEvent, "notExistKey");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void InjectTraceContextIntoBasicProperties_AddsKeyValueToMetadata()
    {
        // Arrange 
        var domainEvent = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "Test",
            User = "usrtest",
            Age = 25
        };

        // Act
        ActivitySourceService.InjectTraceContextIntoBasicProperties(domainEvent, "traceKey", "traceValue");

        // Assert
        Assert.True(domainEvent.Metadata.ContainsKey("traceKey"));
        Assert.Equal("traceValue", domainEvent.Metadata["traceKey"]);
    }

    [Fact]
    public void ActivitySourceService_ActivitySource_ShouldBeInitialized()
    {
        // Arrange
        var service = new ActivitySourceService();

        // Act
        var activitySource = ((IActivityService)service).ActivitySource;

        // Assert
        Assert.NotNull(activitySource);
        Assert.Equal(service.ActivitySource, activitySource);
    }
}
