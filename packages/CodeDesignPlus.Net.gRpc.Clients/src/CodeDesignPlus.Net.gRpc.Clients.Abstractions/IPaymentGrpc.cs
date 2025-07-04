using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to manage payment-related operations.
/// </summary>
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
    /// Updates the status of a payment.
    /// This method is used to update the status of a payment after it has been processed.
    /// It is typically called by the payment gateway or service to notify the system of the payment status change.
    /// </summary>
    /// <param name="id">The unique identifier of the payment to update.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task<UpdateStatusResponse> UpdateStatusAsync(Guid id, CancellationToken cancellationToken);
}
