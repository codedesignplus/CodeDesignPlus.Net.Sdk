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
    public async Task InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken)
    {
        await client.InitiatePaymentAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }
}
