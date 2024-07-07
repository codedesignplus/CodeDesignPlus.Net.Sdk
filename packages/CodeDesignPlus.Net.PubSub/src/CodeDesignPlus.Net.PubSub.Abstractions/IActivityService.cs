using CodeDesignPlus.Net.Core.Abstractions;
using OpenTelemetry.Context.Propagation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDesignPlus.Net.PubSub.Abstractions
{
    public interface IActivityService
    {
        ActivitySource ActivitySource { get; }
        Activity StartActivity(string name, ActivityKind kind, PropagationContext? propagationContext = null);
        bool AddActivity(int id, Activity activity);
        bool TryGetActivity(int id, out Activity activity);
        bool RemoveActivity(int id, out Activity activity);

        void Inject(Activity activity, IDomainEvent domainEvent);
        PropagationContext Extract(IDomainEvent domainEvent);
    }
}
