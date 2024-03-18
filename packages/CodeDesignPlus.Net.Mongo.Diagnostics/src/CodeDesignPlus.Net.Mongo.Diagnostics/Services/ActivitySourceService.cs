using CodeDesignPlus.Net.Mongo.Diagnostics.Subscriber;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CodeDesignPlus.Net.Mongo.Diagnostics.Services
{
    public class ActivitySourceService : IActivityService
    {
        public readonly ActivitySource ActivitySource;

        private readonly ConcurrentDictionary<int, Activity> activityMap = new();

        public ActivitySourceService()
        {
            var assemblyName = typeof(DiagnosticsActivityEventSubscriber).Assembly.GetName();

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

        public Activity StartActivity(string name, ActivityKind kind)
        {
            return this.ActivitySource.StartActivity(name, kind);
        }

        public bool TryGetActivity(int id, out Activity activity)
        {
            return this.activityMap.TryGetValue(id, out activity);
        }
    }
}
