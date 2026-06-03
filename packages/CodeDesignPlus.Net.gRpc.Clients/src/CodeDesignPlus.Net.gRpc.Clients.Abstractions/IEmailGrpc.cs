using CodeDesignPlus.Net.Microservice.Emails.gRpc;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to manage email-related operations.
/// </summary>
public interface IEmailGrpc
{
    /// <summary>
    /// Sends an email using a template or direct content.
    /// </summary>
    /// <param name="request">The request containing email information (recipients, subject, body, attachments, etc.).</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation with the send response.</returns>
    Task<SendEmailResponse> SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken);
}
