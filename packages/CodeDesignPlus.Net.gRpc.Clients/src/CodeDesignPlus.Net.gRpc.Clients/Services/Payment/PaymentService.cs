using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

/// <summary>
/// Service to manage payment-related operations.
/// </summary>
/// <remarks>
/// <para>
/// The <c>Authorization</c> header is sent only when the caller has an access token. A worker, a bus consumer or a
/// recurring job has none, and it used to go out as an empty <c>"Bearer "</c> that ms-payments rejected; without the
/// header the call is a service-to-service call inside the cluster, the same as the one ms-filestorage accepts
/// (Kappali pendings/260 and 660). ms-payments decides which operations accept it.
/// </para>
/// <para>
/// The payout and merchant account operations take the tenant from the request, never from the context: in a
/// recurring job there is no current tenant.
/// </para>
/// </remarks>
/// <param name="client">The gRPC client for payment operations.</param>
/// <param name="userContext">The user context to access user-related information.</param>
public class PaymentService(Payment.PaymentClient client, IUserContext userContext) : IPaymentGrpc
{
    /// <summary>
    /// Initiates a payment process.
    /// </summary>
    /// <param name="request">The request containing payment information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    public async Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken)
    {
        var response = await client.InitiatePaymentAsync(request, BuildHeaders(userContext.Tenant.ToString()), cancellationToken: cancellationToken);

        return response;
    }

    /// <summary>
    /// Gets the current payment status by payment ID.
    /// </summary>
    /// <param name="paymentId">The unique identifier of the payment.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the payment status response.</returns>
    public async Task<GetPaymentStatusResponse> GetPaymentStatusAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var request = new GetPaymentStatusRequest { Id = paymentId.ToString() };

        var response = await client.GetPaymentStatusAsync(request, BuildHeaders(userContext.Tenant.ToString()), cancellationToken: cancellationToken);

        return response;
    }

    /// <summary>
    /// Gets the collection account of a tenant, without its secrets.
    /// </summary>
    /// <param name="request">The request with the tenant whose account is queried.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the merchant account of the tenant.</returns>
    /// <exception cref="ArgumentException">Thrown when the request does not carry a tenant.</exception>
    public async Task<GetMerchantAccountResponse> GetMerchantAccountAsync(GetMerchantAccountRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Tenant);

        return await client.GetMerchantAccountAsync(request, BuildHeaders(request.Tenant), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Requests a payment to a third party from the tenant's collection account.
    /// </summary>
    /// <param name="request">The request with the payout, its tenant and who asked for it.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the payout id and its initial status.</returns>
    /// <exception cref="ArgumentException">Thrown when the request does not carry a tenant.</exception>
    public async Task<RequestPayoutResponse> RequestPayoutAsync(RequestPayoutRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Tenant);

        return await client.RequestPayoutAsync(request, BuildHeaders(request.Tenant), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a payout by its id.
    /// </summary>
    /// <param name="request">The request with the payout id and its tenant.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the payout and its current status.</returns>
    /// <exception cref="ArgumentException">Thrown when the request does not carry a tenant.</exception>
    public async Task<GetPayoutResponse> GetPayoutAsync(GetPayoutRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Tenant);

        return await client.GetPayoutAsync(request, BuildHeaders(request.Tenant), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Builds the call headers: the caller's token when there is one, and the given tenant.
    /// </summary>
    private Grpc.Core.Metadata BuildHeaders(string tenant)
    {
        var headers = new Grpc.Core.Metadata();

        var accessToken = userContext.AccessToken;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            headers.Add("Authorization", $"Bearer {accessToken}");
        }

        headers.Add("X-Tenant", tenant);

        return headers;
    }
}
