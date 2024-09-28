namespace CodeDesignPlus.Net.Mongo.Diagnostics.Abstractions
{
    /// <summary>
    /// Provides an interface for managing activities in a diagnostic context.
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// Gets the <see cref="ActivitySource"/> used to create and manage activities.
        /// </summary>
        ActivitySource ActivitySource { get; }

        /// <summary>
        /// Starts a new activity with the specified name and kind.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <param name="kind">The kind of the activity.</param>
        /// <returns>The started <see cref="Activity"/>.</returns>
        Activity StartActivity(string name, ActivityKind kind);

        /// <summary>
        /// Adds an activity to the service with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <param name="activity">The activity to add.</param>
        /// <returns><c>true</c> if the activity was added successfully; otherwise, <c>false</c>.</returns>
        bool AddActivity(int id, Activity activity);

        /// <summary>
        /// Tries to get an activity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <param name="activity">When this method returns, contains the activity associated with the specified identifier, if the activity is found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the activity was found; otherwise, <c>false</c>.</returns>
        bool TryGetActivity(int id, out Activity activity);

        /// <summary>
        /// Removes an activity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <param name="activity">When this method returns, contains the removed activity, if the activity is found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the activity was removed successfully; otherwise, <c>false</c>.</returns>
        bool RemoveActivity(int id, out Activity activity);
    }
}