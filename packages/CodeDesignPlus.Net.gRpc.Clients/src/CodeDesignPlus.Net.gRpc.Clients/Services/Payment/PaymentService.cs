using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

/// <summary>
/// Service to manage payment-related operations.
/// </summary>
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
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken)
    {
        return await client.InitiatePaymentAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates the status of a payment.
    /// This method is used to update the status of a payment after it has been processed.
    /// It is typically called by the payment gateway or service to notify the system of the payment status change.
    /// </summary>
    /// <param name="id">The unique identifier of the payment to update.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    public async Task<UpdateStatusResponse> UpdateStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new UpdateStatusRequest { Id = id.ToString() };

        return await client.UpdateStatusAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }
}
