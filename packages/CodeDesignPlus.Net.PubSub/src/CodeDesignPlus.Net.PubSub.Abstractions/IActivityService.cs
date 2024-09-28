namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Interface for managing activities within the PubSub system.
/// </summary>
public interface IActivityService
{
    /// <summary>
    /// Gets the activity source used for creating activities.
    /// </summary>
    ActivitySource ActivitySource { get; }

    /// <summary>
    /// Starts a new activity with the specified name and kind.
    /// </summary>
    /// <param name="name">The name of the activity.</param>
    /// <param name="kind">The kind of activity.</param>
    /// <param name="propagationContext">Optional propagation context for the activity.</param>
    /// <returns>The started activity.</returns>
    Activity StartActivity(string name, ActivityKind kind, PropagationContext? propagationContext = null);

    /// <summary>
    /// Adds an activity to the service with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the activity.</param>
    /// <param name="activity">The activity to add.</param>
    /// <returns>True if the activity was added successfully; otherwise, false.</returns>
    bool AddActivity(int id, Activity activity);

    /// <summary>
    /// Tries to get an activity with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the activity.</param>
    /// <param name="activity">When this method returns, contains the activity associated with the specified ID, if the ID is found; otherwise, null.</param>
    /// <returns>True if the activity was found; otherwise, false.</returns>
    bool TryGetActivity(int id, out Activity activity);

    /// <summary>
    /// Removes an activity with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the activity.</param>
    /// <param name="activity">When this method returns, contains the removed activity, if the activity was removed; otherwise, null.</param>
    /// <returns>True if the activity was removed successfully; otherwise, false.</returns>
    bool RemoveActivity(int id, out Activity activity);

    /// <summary>
    /// Injects the specified activity into the given domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="activity">The activity to inject.</param>
    /// <param name="domainEvent">The domain event to inject the activity into.</param>
    void Inject<TDomainEvent>(Activity activity, TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Extracts the propagation context from the given domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event to extract the propagation context from.</param>
    /// <returns>The extracted propagation context.</returns>
    PropagationContext Extract<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
}