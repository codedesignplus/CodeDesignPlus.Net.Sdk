using CodeDesignPlus.Net.Core.Abstractions;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace CodeDesignPlus.Net.PubSub.Abstractions
{
    public interface IActivityService
    {
        ActivitySource ActivitySource { get; }
        Activity StartActivity(string name, ActivityKind kind, PropagationContext? propagationContext = null);
        bool AddActivity(int id, Activity activity);
        bool TryGetActivity(int id, out Activity activity);
        bool RemoveActivity(int id, out Activity activity);

        void Inject<TDomainEvent>(Activity activity, TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
        PropagationContext Extract<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
    }
}
