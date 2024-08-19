namespace CodeDesignPlus.Net.PubSub.Diagnostics;

/// <summary>
/// Provides services for managing activity sources and context propagation.
/// </summary>
/// <remarks>
/// https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/examples/MicroserviceExample/Utils/Messaging/MessageReceiver.cs
/// </remarks>
public class ActivitySourceService : IActivityService
{
    /// <summary>
    /// The activity source used for creating activities.
    /// </summary>
    public readonly ActivitySource ActivitySource;

    private readonly ConcurrentDictionary<int, Activity> activityMap = new();

    private readonly TextMapPropagator propagator = Propagators.DefaultTextMapPropagator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivitySourceService"/> class.
    /// </summary>
    public ActivitySourceService()
    {
        var assemblyName = typeof(ActivitySourceService).Assembly.GetName();
        this.ActivitySource = new(assemblyName.Name, assemblyName.Version.ToString());
    }

    /// <summary>
    /// Gets the activity source.
    /// </summary>
    ActivitySource IActivityService.ActivitySource => this.ActivitySource;

    /// <summary>
    /// Adds an activity to the activity map.
    /// </summary>
    /// <param name="id">The identifier of the activity.</param>
    /// <param name="activity">The activity to add.</param>
    /// <returns>True if the activity was added successfully; otherwise, false.</returns>
    public bool AddActivity(int id, Activity activity)
    {
        return this.activityMap.TryAdd(id, activity);
    }

    /// <summary>
    /// Removes an activity from the activity map.
    /// </summary>
    /// <param name="id">The identifier of the activity.</param>
    /// <param name="activity">The removed activity.</param>
    /// <returns>True if the activity was removed successfully; otherwise, false.</returns>
    public bool RemoveActivity(int id, out Activity activity)
    {
        return this.activityMap.TryRemove(id, out activity);
    }

    /// <summary>
    /// Starts a new activity.
    /// </summary>
    /// <param name="name">The name of the activity.</param>
    /// <param name="kind">The kind of the activity.</param>
    /// <param name="propagationContext">The propagation context.</param>
    /// <returns>The started activity.</returns>
    public Activity StartActivity(string name, ActivityKind kind, PropagationContext? propagationContext = null)
    {
        if (propagationContext.HasValue)
            return this.ActivitySource.StartActivity(name, kind, propagationContext.Value.ActivityContext);

        return this.ActivitySource.StartActivity(name, kind);
    }

    /// <summary>
    /// Tries to get an activity from the activity map.
    /// </summary>
    /// <param name="id">The identifier of the activity.</param>
    /// <param name="activity">The retrieved activity.</param>
    /// <returns>True if the activity was found; otherwise, false.</returns>
    public bool TryGetActivity(int id, out Activity activity)
    {
        return this.activityMap.TryGetValue(id, out activity);
    }

    /// <summary>
    /// Injects the trace context into the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="activity">The activity containing the trace context.</param>
    /// <param name="domainEvent">The domain event.</param>
    public void Inject<TDomainEvent>(Activity activity, TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        ActivityContext contextToInject = default;

        if (activity != null)
        {
            contextToInject = activity.Context;
        }
        else if (Activity.Current != null)
        {
            contextToInject = Activity.Current.Context;
        }

        this.propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), domainEvent, InjectTraceContextIntoBasicProperties);
    }

    /// <summary>
    /// Extracts the trace context from the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event.</param>
    /// <returns>The extracted propagation context.</returns>
    public PropagationContext Extract<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var parentContext = this.propagator.Extract(default, domainEvent, ExtractTraceContextFromBasicProperties);
        Baggage.Current = parentContext.Baggage;
        return parentContext;
    }

    /// <summary>
    /// Extracts the trace context from the basic properties of the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="event">The domain event.</param>
    /// <param name="key">The key of the trace context.</param>
    /// <returns>The extracted trace context values.</returns>
    internal static IEnumerable<string> ExtractTraceContextFromBasicProperties<TDomainEvent>(TDomainEvent @event, string key) where TDomainEvent : IDomainEvent
    {
        if (@event.Metadata.TryGetValue(key, out var value))
        {
            return new[] { value.ToString() };
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Injects the trace context into the basic properties of the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event.</param>
    /// <param name="key">The key of the trace context.</param>
    /// <param name="value">The value of the trace context.</param>
    internal static void InjectTraceContextIntoBasicProperties<TDomainEvent>(TDomainEvent domainEvent, string key, string value) where TDomainEvent : IDomainEvent
    {
        Console.WriteLine($"Injecting {key}: {value}");
        domainEvent.Metadata.Add(key, value);
    }
}