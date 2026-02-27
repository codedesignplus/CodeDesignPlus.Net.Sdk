using System;
using CodeDesignPlus.Net.gRpc.Clients.Services.Countries;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service interface for country-related operations.
/// </summary>
public interface ICountryGrpc
{
    /// <summary>
    /// Retrieves country information.
    /// </summary>
    /// <param name="request">The request containing country information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    Task<GetCountryResponse> GetCountryAsync(GetCountryRequest request, CancellationToken cancellationToken);

    
    /// <summary>
    /// Retrieves country information.
    /// </summary>
    /// <param name="request">The request containing country information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    Task<GetCountryResponse> GetCountryAsync(Guid? id = null, string code = null, string name = null, string alpha2 = null, string alpha3 = null, CancellationToken cancellationToken = default);
}
