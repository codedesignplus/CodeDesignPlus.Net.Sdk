namespace CodeDesignPlus.Net.Mongo.Diagnostics.Services
{
    /// <summary>
    /// Service for managing activities in a diagnostic context using <see cref="ActivitySource"/>.
    /// </summary>
    public class ActivitySourceService : IActivityService
    {
        /// <summary>
        /// The <see cref="ActivitySource"/> used to create and manage activities.
        /// </summary>
        public readonly ActivitySource ActivitySource;

        private readonly ConcurrentDictionary<int, Activity> activityMap = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitySourceService"/> class.
        /// </summary>
        public ActivitySourceService()
        {
            var assemblyName = typeof(DiagnosticsActivityEventSubscriber).Assembly.GetName();
            this.ActivitySource = new(assemblyName.Name, assemblyName.Version.ToString());
        }

        /// <summary>
        /// Gets the <see cref="ActivitySource"/> used to create and manage activities.
        /// </summary>
        ActivitySource IActivityService.ActivitySource => this.ActivitySource;

        /// <summary>
        /// Adds an activity to the service with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <param name="activity">The activity to add.</param>
        /// <returns><c>true</c> if the activity was added successfully; otherwise, <c>false</c>.</returns>
        public bool AddActivity(int id, Activity activity)
        {
            return this.activityMap.TryAdd(id, activity);
        }

        /// <summary>
        /// Removes an activity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <param name="activity">When this method returns, contains the removed activity, if the activity is found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the activity was removed successfully; otherwise, <c>false</c>.</returns>
        public bool RemoveActivity(int id, out Activity activity)
        {
            return this.activityMap.TryRemove(id, out activity);
        }

        /// <summary>
        /// Starts a new activity with the specified name and kind.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="kind">The kind of the activity.</param>
        /// <returns>The started <see cref="Activity"/>.</returns>
        public Activity StartActivity(string name, ActivityKind kind)
        {
            return this.ActivitySource.StartActivity(name, kind);
        }

        /// <summary>
        /// Tries to get an activity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <param name="activity">When this method returns, contains the activity associated with the specified identifier, if the activity is found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the activity was found; otherwise, <c>false</c>.</returns>
        public bool TryGetActivity(int id, out Activity activity)
        {
            return this.activityMap.TryGetValue(id, out activity);
        }
    }
}