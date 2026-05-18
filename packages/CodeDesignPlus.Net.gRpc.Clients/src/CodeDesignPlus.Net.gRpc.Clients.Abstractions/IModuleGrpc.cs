using CodeDesignPlus.Net.Microservice.Modules.gRpc;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Abstraction for the Module gRPC client.
/// Used to manage modules (create, update, query) from other microservices.
/// </summary>
public interface IModuleGrpc
{
    /// <summary>
    /// Creates a new module if it does not already exist.
    /// </summary>
    Task CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing module.
    /// </summary>
    Task UpdateModuleAsync(UpdateModuleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a module by ID.
    /// </summary>
    Task DeleteModuleAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a module by its ID. Returns null if not found.
    /// </summary>
    Task<GetModuleResponse> GetModuleByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all modules with optional filtering.
    /// </summary>
    Task<GetAllModulesResponse> GetAllModulesAsync(string filters = null, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a service to an existing module.
    /// </summary>
    Task AddServiceAsync(AddServiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a service from a module.
    /// </summary>
    Task RemoveServiceAsync(string moduleId, string serviceId, CancellationToken cancellationToken = default);
}
