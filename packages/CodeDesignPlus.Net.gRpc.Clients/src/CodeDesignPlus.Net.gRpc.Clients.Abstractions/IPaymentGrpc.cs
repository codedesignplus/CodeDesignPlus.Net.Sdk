using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to manage payment-related operations.
/// </summary>
/// <remarks>
/// Every call forwards the caller's access token when there is one. Without it —a worker, a bus consumer or a
/// recurring job— the call goes out with no <c>Authorization</c> header at all, as a service-to-service call inside
/// the cluster. The payout and merchant account operations carry the tenant in the request for that reason.
/// </remarks>
public interface IPaymentGrpc
{
    /// <summary>
    /// Initiates a payment process.
    /// </summary>
    /// <param name="request">The request containing payment information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the current payment status by payment ID.
    /// </summary>
    /// <param name="paymentId">The unique identifier of the payment.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the payment status response.</returns>
    Task<GetPaymentStatusResponse> GetPaymentStatusAsync(Guid paymentId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the collection account of a tenant, without its secrets.
    /// </summary>
    /// <param name="request">The request with the tenant whose account is queried.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the merchant account of the tenant.</returns>
    Task<GetMerchantAccountResponse> GetMerchantAccountAsync(GetMerchantAccountRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Requests a payment to a third party from the tenant's collection account.
    /// </summary>
    /// <param name="request">The request with the payout, its tenant and who asked for it.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the payout id and its initial status.</returns>
    Task<RequestPayoutResponse> RequestPayoutAsync(RequestPayoutRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a payout by its id.
    /// </summary>
    /// <param name="request">The request with the payout id and its tenant.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the payout and its current status.</returns>
    Task<GetPayoutResponse> GetPayoutAsync(GetPayoutRequest request, CancellationToken cancellationToken);
}
