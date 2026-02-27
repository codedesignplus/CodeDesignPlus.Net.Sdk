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
}
