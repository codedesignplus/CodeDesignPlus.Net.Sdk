using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

/// <summary>
/// Service to manage payment-related operations.
/// </summary>
/// <param name="client">The gRPC client for payment operations.</param>
/// <param name="httpContextAccessor">The HTTP context accessor to retrieve request information.</param>
/// <param name="userContext">The user context to access user-related information.</param>
/// <param name="logger">The logger for logging operations.</param>
public class PaymentService(CodeDesignPlus.Net.gRpc.Clients.Services.Payment.Payment.PaymentClient client, IHttpContextAccessor httpContextAccessor, IUserContext userContext, ILogger<PaymentService> logger) : IPaymentGrpc
{
    /// <summary>
    /// Initiates a payment process.
    /// </summary>
    /// <param name="request">The request containing payment information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task PayAsync(PayRequest request, CancellationToken cancellationToken)
    {
        request.Transaction.DeviceSessionId = userContext.IdUser.ToString();
        request.Transaction.IpAddress = userContext.IpAddress;
        request.Transaction.UserAgent = userContext.UserAgent;
        request.Transaction.Cookie = httpContextAccessor.HttpContext?.Request.Cookies["PaymentCookie"] ?? Guid.NewGuid().ToString();
        
        logger.LogInformation("Processing payment for user {UserId} with Tenant {TenantId} from IP {IpAddress}", userContext.IdUser, userContext.Tenant, userContext.IpAddress);

        logger.LogDebug("Payment request details: {@Request}", request);

        await client.PayAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves payment information by ID.
    /// </summary>
    /// <param name="request">The request containing the payment ID.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation with the payment information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task<PaymentResponse> GetPayByIdAsync(GetPaymentRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing payment retrieval for user {UserId} with Tenant {TenantId}", userContext.IdUser, userContext.Tenant);

        logger.LogDebug("Payment retrieval request details: {@Request}", request);

        var response = await client.GetPaymentAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);

        return response;
    }

}
