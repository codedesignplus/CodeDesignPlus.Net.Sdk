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
    /// Retrieves currency information based on the provided parameters.
    /// </summary>
    /// <param name="id">The unique identifier of the currency.</param>
    /// <param name="code">The unique code of the currency.</param>
    /// <param name="numericCode">The numeric code of the currency.</param>
    /// <param name="name">The name of the currency.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public Task<GetCurrencyResponse> GetCurrencyAsync(Guid? id = null, string? code = null, int? numericCode = null, string? name = null, CancellationToken cancellationToken = default)
    {
        var request = new GetCurrencyRequest
        {
            Id = id?.ToString() ?? null,
            Code = code ?? string.Empty,
            NumericCode = numericCode ?? 0,
            Name = name ?? string.Empty
        };

        return GetCurrencyAsync(request, cancellationToken);
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
