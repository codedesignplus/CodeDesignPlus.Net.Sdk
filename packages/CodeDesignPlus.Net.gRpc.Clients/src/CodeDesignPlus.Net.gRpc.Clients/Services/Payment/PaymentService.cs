using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

/// <summary>
/// Service to manage payment-related operations.
/// </summary>
/// <param name="client">The gRPC client for payment operations.</param>
/// <param name="httpContextAccessor">The HTTP context accessor to retrieve request information.</param>
public class PaymentService(CodeDesignPlus.Net.gRpc.Clients.Services.Payment.Payment.PaymentClient client, IHttpContextAccessor httpContextAccessor) : IPaymentGrpc
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
        request.Transaction.DeviceSessionId = httpContextAccessor.HttpContext?.Session.Id ?? Guid.NewGuid().ToString();
        request.Transaction.IpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        request.Transaction.UserAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "CodeDesignPlus/Client-gRpc";
        request.Transaction.Cookie = httpContextAccessor.HttpContext?.Request.Cookies["PaymentCookie"] ?? Guid.NewGuid().ToString();

        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        await client.PayAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
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
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        var response = await client.GetPaymentAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);

        return response;
    }

}
