using CodeDesignPlus.Net.Microservice.Emails.gRpc;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Emails;

/// <summary>
/// Service to manage email-related operations.
/// </summary>
/// <param name="client">The gRPC client for email operations.</param>
/// <param name="userContext">The user context to access user-related information.</param>
public class EmailService(CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient client, IUserContext userContext) : IEmailGrpc
{
    /// <summary>
    /// Sends an email using a template or direct content.
    /// </summary>
    /// <param name="request">The request containing email information (recipients, subject, body, attachments, etc.).</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation with the send response.</returns>
    public async Task<SendEmailResponse> SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken)
    {
        var response = await client.SendEmailAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);

        return response;
    }
}
