using CodeDesignPlus.Net.gRpc.Clients.Services.Memory;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Currencies;

/// <summary>
/// Service to manage currency-related operations.
/// </summary>
/// <param name="client">The gRPC client for currency operations.</param>
/// <param name="memoryService">The memory service for caching currency data.</param>
public class CurrenciesService(CurrencyService.CurrencyServiceClient client, IMemoryService<GetCurrencyResponse> memoryService) : ICurrencyGrpc
{
    /// <summary>
    /// Retrieves currency information.
    /// </summary>
    /// <param name="request">The request containing currency information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task<GetCurrencyResponse> GetCurrencyAsync(GetCurrencyRequest request, CancellationToken cancellationToken)
    {
        var cachedResponse = memoryService.GetMemory(GetKey(request));

        if (cachedResponse != null)
            return cachedResponse;
        
        var response = await client.GetCurrencyAsync(request, cancellationToken: cancellationToken);

        memoryService.AddMemory(GetKey(request), response);

        return response;
    }

    /// <summary>
    /// Generates a unique key for caching currency data based on the request parameters.
    /// </summary>
    /// <param name="request">The request containing currency information.</param>
    /// <returns>Returns a unique key for caching.</returns>
    private static string GetKey(GetCurrencyRequest request)
    {
        return $"Currency:{request.Id}:{request.Name}:{request.Code}:{request.NumericCode}";
    }
}
