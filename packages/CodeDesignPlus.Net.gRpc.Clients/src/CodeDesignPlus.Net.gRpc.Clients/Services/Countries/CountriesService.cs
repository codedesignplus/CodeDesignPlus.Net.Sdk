using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.gRpc.Clients.Services.Memory;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Countries;

/// <summary>
/// Service to manage country-related operations.
/// </summary>
/// <param name="client">The gRPC client for country operations.</param>
/// <param name="memoryService">The memory service for caching country data.</param>
public class CountriesService(CountryService.CountryServiceClient client, IMemoryService<Country> memoryService) : ICountryGrpc
{
    /// <summary>
    /// Retrieves country information.
    /// </summary>
    /// <param name="request">The request containing country information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task<Country> GetCountryAsync(GetCountryRequest request, CancellationToken cancellationToken)
    {
        var cachedResponse = memoryService.GetMemory(GetKey(request));

        if (cachedResponse != null)
            return cachedResponse;
        
        var response = await client.GetCountryAsync(request, cancellationToken: cancellationToken);

        Guard.IsNull(response, Net.Exceptions.Layer.None, "000 : Country not found.");
        Guard.IsFalse(Guid.TryParse(response.Id, out var countryId), Net.Exceptions.Layer.None, "001 : Invalid country ID.");        
        Guard.IsFalse(ushort.TryParse(response.Code, out var countryCode), Net.Exceptions.Layer.None, "002 : Invalid country code.");
        Guard.IsFalse(Guid.TryParse(response.Currency.Id, out var currencyId), Net.Exceptions.Layer.None, "003 : Invalid currency ID.");

        var currency = ValueObjects.Financial.Currency.Create(currencyId, response.Currency.Name, response.Currency.Code, response.Currency.Symbol, (short)response.Currency.DecimalDigits, (short)response.Currency.NumericCode);

        var country = Country.Create(countryId, response.Name, response.Alpha2, response.Alpha3, countryCode, response.Timezone, currency);

        memoryService.AddMemory(GetKey(request), country);

        return country;
    }

    /// <summary>
    /// Retrieves country information based on the provided parameters.
    /// </summary>
    /// <param name="id">The unique identifier of the country.</param>
    /// <param name="code">The unique code of the country.</param>
    /// <param name="name">The name of the country.</param>
    /// <param name="alpha2">The alpha-2 code of the country.</param>
    /// <param name="alpha3">The alpha-3 code of the country.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    public Task<Country> GetCountryAsync(Guid? id = null, string? code = null, string? name = null, string? alpha2 = null, string? alpha3 = null, CancellationToken cancellationToken = default)
    {
        var request = new GetCountryRequest
        {
            Id = id?.ToString() ?? null,
            Code = code ?? string.Empty,
            Name = name ?? string.Empty,
            Alpha2 = alpha2 ?? string.Empty,
            Alpha3 = alpha3 ?? string.Empty
        };

        return GetCountryAsync(request, cancellationToken);
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
