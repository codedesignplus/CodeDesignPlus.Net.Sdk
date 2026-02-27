using CodeDesignPlus.Net.gRpc.Clients.Services.Memory;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Countries;

/// <summary>
/// Service to manage country-related operations.
/// </summary>
/// <param name="client">The gRPC client for country operations.</param>
/// <param name="memoryService">The memory service for caching country data.</param>
public class CountriesService(CountryService.CountryServiceClient client, IMemoryService<GetCountryResponse> memoryService) : ICountryGrpc
{
    /// <summary>
    /// Retrieves country information.
    /// </summary>
    /// <param name="request">The request containing country information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task<GetCountryResponse> GetCountryAsync(GetCountryRequest request, CancellationToken cancellationToken)
    {
        var cachedResponse = memoryService.GetMemory(GetKey(request));

        if (cachedResponse != null)
            return cachedResponse;
        
        var response = await client.GetCountryAsync(request, cancellationToken: cancellationToken);

        memoryService.AddMemory(GetKey(request), response);

        return response;
    }

    /// <summary>
    /// Generates a unique key for caching country data based on the request parameters.
    /// </summary>
    /// <param name="request">The request containing country information.</param>
    /// <returns>Returns a unique key for caching.</returns>
    private static string GetKey(GetCountryRequest request)
    {
        return $"Country:{request.Id}:{request.Name}:{request.Code}:{request.Alpha2}:{request.Alpha3}";
    }
}
