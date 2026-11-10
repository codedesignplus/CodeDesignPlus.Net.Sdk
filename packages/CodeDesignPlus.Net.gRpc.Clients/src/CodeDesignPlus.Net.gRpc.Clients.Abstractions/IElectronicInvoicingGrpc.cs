using CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Client of the gRPC contract of ms-electronicinvoicing: issues electronic documents through the provider of the tax
/// authority and reads them back.
/// </summary>
/// <remarks>
/// <para>
/// The issuer is decided only by the request: a tenant in <c>issuer_tenant</c> (or <c>tenant</c>) is the issuer of that
/// tenant, and none with <c>platform_issuer = true</c> is the platform's own issuer, the company that runs the platform
/// and invoices what it sells (Kappali pendings/661 and 663). Sending both is refused by the server with
/// <c>INVALID_ARGUMENT</c>.
/// </para>
/// <para>
/// The callers are workers, bus consumers and jobs without a user token, so no header is sent: neither
/// <c>Authorization</c> nor <c>X-Tenant</c>. The server never reads the tenant from the context, and a header taken from
/// the caller's context could only contradict the request.
/// </para>
/// <para>
/// Amounts travel in minor units of their currency and rates in basis points. The answer to an issue is immediate
/// (<c>DRAFT</c>); the result arrives later by the events <c>ElectronicDocumentValidatedDomainEvent</c> and
/// <c>ElectronicDocumentRejectedDomainEvent</c>, whose tenant is the reserved one of the platform for the documents of
/// the platform's issuer: correlate them by the document id or by <c>reference</c>.
/// </para>
/// </remarks>
public interface IElectronicInvoicingGrpc
{
    /// <summary>
    /// Issues a document. The same id with the same content returns the same document, so a retry does not issue twice;
    /// the same id with another content is refused with <c>ALREADY_EXISTS</c>.
    /// </summary>
    /// <param name="request">The document, with its id derived by the caller from its own source.</param>
    /// <param name="cancellationToken">Cancellation token propagated to the gRPC call.</param>
    /// <returns>The document as it stands right after being accepted.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the request has no id.</exception>
    Task<IssueDocumentResponse> IssueDocumentAsync(IssueDocumentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a document of a tenant or of the platform's issuer.
    /// </summary>
    /// <param name="request">The id of the document and its scope.</param>
    /// <param name="cancellationToken">Cancellation token propagated to the gRPC call.</param>
    /// <returns>The document with its number, CUFE, status and files.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the request has no id.</exception>
    Task<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Corrects a rejected document with the content sent (same id, same number), or looks at a waiting one now.
    /// </summary>
    /// <param name="request">The id, the scope and, for a rejected document, the corrected content.</param>
    /// <param name="cancellationToken">Cancellation token propagated to the gRPC call.</param>
    /// <returns>The document as it stands.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the request has no id.</exception>
    Task<IssueDocumentResponse> RetryDocumentAsync(RetryDocumentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the issuer of a tenant or the platform's: whether it can issue, what its provider can do and the documents
    /// left in its quota, so the caller can block a charge that cannot be invoiced before collecting it.
    /// </summary>
    /// <param name="request">The scope of the issuer.</param>
    /// <param name="cancellationToken">Cancellation token propagated to the gRPC call.</param>
    /// <returns>The issuer; <c>registered = false</c> when there is none.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    Task<GetIssuerResponse> GetIssuerAsync(GetIssuerRequest request, CancellationToken cancellationToken);
}
