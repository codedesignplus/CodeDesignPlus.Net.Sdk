namespace CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing;

/// <summary>
/// Implementation of <see cref="IElectronicInvoicingGrpc"/> over the generated client of
/// <c>ElectronicInvoicing.ElectronicInvoicing</c>.
/// </summary>
/// <remarks>
/// <para>
/// It does not depend on <c>IUserContext</c>: it is called from workers, bus consumers and jobs, where there is no user
/// token and the tenant of the context, if any, is not the issuer. The scope travels only in the request, so the calls
/// go out without the caller's token or tenant, as service-to-service calls inside the cluster that the server accepts
/// anonymously.
/// </para>
/// <para>
/// The only header is <c>accept-language</c>, when the caller set <see cref="CodeDesignPlus.Net.Exceptions.ErrorLanguage.Current"/>: the server writes
/// the reason of a refusal in that language, and the caller keeps it to show it later.
/// </para>
/// <para>
/// It is not named <c>ElectronicInvoicingService</c> because that is the name of the server's implementation, and a
/// project that references both would have to tell them apart.
/// </para>
/// </remarks>
/// <param name="client">The generated gRPC client of the <c>ElectronicInvoicing.ElectronicInvoicing</c> service.</param>
public class ElectronicInvoicingGrpcService(ElectronicInvoicing.ElectronicInvoicingClient client) : IElectronicInvoicingGrpc
{
    /// <inheritdoc/>
    public async Task<IssueDocumentResponse> IssueDocumentAsync(IssueDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        return await client.IssueDocumentAsync(request, Headers(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        return await client.GetDocumentAsync(request, Headers(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IssueDocumentResponse> RetryDocumentAsync(RetryDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        return await client.RetryDocumentAsync(request, Headers(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GetIssuerResponse> GetIssuerAsync(GetIssuerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await client.GetIssuerAsync(request, Headers(), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// The headers of a call: only the language of the errors, when the caller asked for one.
    /// </summary>
    private static Grpc.Core.Metadata Headers()
    {
        var headers = new Grpc.Core.Metadata();

        if (!string.IsNullOrWhiteSpace(CodeDesignPlus.Net.Exceptions.ErrorLanguage.Current))
        {
            headers.Add("accept-language", CodeDesignPlus.Net.Exceptions.ErrorLanguage.Current);
        }

        return headers;
    }
}
