using CodeDesignPlus.Net.gRpc.Clients.Services.Currencies;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Defines the contract for interacting with currency-related gRPC services.
/// </summary>
public interface ICurrencyGrpc
{
    /// <summary>
    /// Retrieves currency information.
    /// </summary>
    /// <param name="request">The request containing currency information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    Task<GetCurrencyResponse> GetCurrencyAsync(GetCurrencyRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves currency information by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the currency.</param>
    /// <param name="code">The unique code of the currency.</param>
    /// <param name="numericCode">The numeric code of the currency.</param>
    /// <param name="name">The name of the currency.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    Task<GetCurrencyResponse> GetCurrencyAsync(Guid? id = null, string code = null, int? numericCode = null, string name = null, CancellationToken cancellationToken = default);
}
