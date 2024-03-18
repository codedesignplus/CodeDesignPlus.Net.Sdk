using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace CodeDesignPlus.Net.PubSub.Diagnostics
{
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

        public void Inject(Activity activity, IDomainEvent domainEvent)
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

            this.propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), domainEvent, this.InjectTraceContextIntoBasicProperties);
        }

        public PropagationContext Extract(IDomainEvent domainEvent)
        {
            var parentContext = this.propagator.Extract(default, domainEvent, this.ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;

            return parentContext;
        }

        private IEnumerable<string> ExtractTraceContextFromBasicProperties(IDomainEvent @event, string key)
        {

            if (@event.Metadata.TryGetValue(key, out var value))
            {
                return [value.ToString()];
            }

            return [];
        }

        private void InjectTraceContextIntoBasicProperties(IDomainEvent domainEvent, string key, string value)
        {
            domainEvent.Metadata.Add(key, value);
        }
    }
}
