namespace CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing;

/// <summary>
/// Implementation of <see cref="IElectronicInvoicingGrpc"/> over the generated client of
/// <c>ElectronicInvoicing.ElectronicInvoicing</c>.
/// </summary>
/// <remarks>
/// <para>
/// It does not depend on <c>IUserContext</c>: it is called from workers, bus consumers and jobs, where there is no user
/// token and the tenant of the context, if any, is not the issuer. The scope travels only in the request, so the calls
/// go out without headers, as service-to-service calls inside the cluster that the server accepts anonymously.
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

        return await client.IssueDocumentAsync(request, new Grpc.Core.Metadata(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        return await client.GetDocumentAsync(request, new Grpc.Core.Metadata(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IssueDocumentResponse> RetryDocumentAsync(RetryDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        return await client.RetryDocumentAsync(request, new Grpc.Core.Metadata(), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GetIssuerResponse> GetIssuerAsync(GetIssuerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await client.GetIssuerAsync(request, new Grpc.Core.Metadata(), cancellationToken: cancellationToken);
    }
}
