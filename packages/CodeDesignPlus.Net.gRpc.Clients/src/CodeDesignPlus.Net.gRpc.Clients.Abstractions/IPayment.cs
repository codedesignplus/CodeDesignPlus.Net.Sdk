using System;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

public interface IPayment
{
    Task PayAsync(PayRequest request, CancellationToken cancellationToken);
    Task<PaymentResponse> GetPayByIdAsync(GetPaymentRequest request, CancellationToken cancellationToken);
}
