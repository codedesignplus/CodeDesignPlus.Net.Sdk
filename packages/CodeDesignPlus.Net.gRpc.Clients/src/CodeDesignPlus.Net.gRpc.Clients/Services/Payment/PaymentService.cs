using System;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

public class PaymentService(CodeDesignPlus.Net.gRpc.Clients.Services.Payment.Payment.PaymentClient client, IHttpContextAccessor httpContextAccessor) : IPayment
{
    public async Task PayAsync(PayRequest request, CancellationToken cancellationToken)
    {
        request.Transaction.DeviceSessionId = httpContextAccessor.HttpContext?.Session.Id ?? Guid.NewGuid().ToString();
        request.Transaction.IpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "::1";
        request.Transaction.UserAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "CodeDesignPlus/Client-gRpc";
        request.Transaction.Cookie = httpContextAccessor.HttpContext?.Request.Cookies["PaymentCookie"] ?? Guid.NewGuid().ToString();

        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        await client.PayAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);
    }

    public async Task<PaymentResponse> GetPayByIdAsync(GetPaymentRequest request, CancellationToken cancellationToken)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        var response = await client.GetPaymentAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);
    
        return response;
    }

}
