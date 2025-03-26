namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Service to validate the permissions of the user based on the roles assigned to it.
/// </summary>
public interface IRbac
{
    /// <summary>
    /// Load the roles and permissions of the user.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Return a <see cref="Task"/> representing the asynchronous operation.</returns>
    Task LoadRbacAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Validate if the user has permission to access the controller and action.
    /// </summary>
    /// <param name="controller">The name of the controller to validate.</param>
    /// <param name="action">The name of the action to validate.</param>
    /// <param name="httpVerb">The HTTP verb to validate.</param>
    /// <param name="roles">The roles assigned to the user.</param>
    /// <returns>Return true if the user has permission to access the controller and action; otherwise, false.</returns>
    Task<bool> IsAuthorizedAsync(string controller, string action, string httpVerb, string[] roles);
}
