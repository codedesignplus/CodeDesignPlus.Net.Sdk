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
    private Grpc.Core.Metadata GetMetadata() => new()
    {
        { "Authorization", $"Bearer {userContext.AccessToken}" },
        { "X-Tenant", userContext.Tenant.ToString() }
    };

    /// <inheritdoc/>
    public async Task<RenderTemplateResponse> RenderTemplateAsync(RenderTemplateRequest request, CancellationToken cancellationToken)
    {
        return await client.RenderTemplateAsync(request, GetMetadata(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GeneratePdfResponse> GeneratePdfAsync(GeneratePdfRequest request, CancellationToken cancellationToken)
    {
        return await client.GeneratePdfAsync(request, GetMetadata(), cancellationToken: cancellationToken);
    }
}
