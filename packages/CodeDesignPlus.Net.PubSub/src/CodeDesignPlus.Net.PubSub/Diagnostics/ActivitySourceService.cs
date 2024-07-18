//https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/examples/MicroserviceExample/Utils/Messaging/MessageReceiver.cs
namespace CodeDesignPlus.Net.PubSub.Diagnostics;

public class ActivitySourceService : IActivityService
{
    public readonly ActivitySource ActivitySource;

    private readonly ConcurrentDictionary<int, Activity> activityMap = new();

    private readonly TextMapPropagator propagator = Propagators.DefaultTextMapPropagator;

    public ActivitySourceService()
    {
        var assemblyName = typeof(ActivitySourceService).Assembly.GetName();

        this.ActivitySource = new(assemblyName.Name, assemblyName.Version.ToString());
    }

    ActivitySource IActivityService.ActivitySource => this.ActivitySource;

    public bool AddActivity(int id, Activity activity)
    {
        return this.activityMap.TryAdd(id, activity);
    }

    public bool RemoveActivity(int id, out Activity activity)
    {
        return this.activityMap.TryRemove(id, out activity);
    }

    public Activity StartActivity(string name, ActivityKind kind, PropagationContext? propagationContext = null)
    {
        if (propagationContext.HasValue)
            return this.ActivitySource.StartActivity(name, kind, propagationContext.Value.ActivityContext);

        return this.ActivitySource.StartActivity(name, kind);
    }


    public bool TryGetActivity(int id, out Activity activity)
    {
        return this.activityMap.TryGetValue(id, out activity);
    }

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

    public PropagationContext Extract<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
    {
        var parentContext = this.propagator.Extract(default, domainEvent, ExtractTraceContextFromBasicProperties);

        Baggage.Current = parentContext.Baggage;

        return parentContext;
    }

    internal static IEnumerable<string> ExtractTraceContextFromBasicProperties<TDomainEvent>(TDomainEvent @event, string key) where TDomainEvent : IDomainEvent
    {

        if (@event.Metadata.TryGetValue(key, out var value))
        {
            return [value.ToString()];
        }

        return [];
    }

    internal static void InjectTraceContextIntoBasicProperties<TDomainEvent>(TDomainEvent domainEvent, string key, string value) where TDomainEvent : IDomainEvent
    {

        Console.WriteLine($"Injecting {key}: {value}");
        domainEvent.Metadata.Add(key, value);
    }
}

