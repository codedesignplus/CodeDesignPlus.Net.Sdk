using CodeDesignPlus.Net.Microservice.Emails.gRpc;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to manage email-related operations.
/// </summary>
public interface IEmailGrpc
{
    /// <summary>
    /// Renders an email template with the provided variable values.
    /// </summary>
    /// <param name="request">The request containing the template type and variable values.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the rendered HTML content and subject.</returns>
    Task<RenderTemplateResponse> RenderTemplateAsync(RenderTemplateRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Generates a PDF from a template with the provided variable values.
    /// </summary>
    /// <param name="request">The request containing the template type and variable values.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns the generated PDF bytes.</returns>
    Task<GeneratePdfResponse> GeneratePdfAsync(GeneratePdfRequest request, CancellationToken cancellationToken);
}
