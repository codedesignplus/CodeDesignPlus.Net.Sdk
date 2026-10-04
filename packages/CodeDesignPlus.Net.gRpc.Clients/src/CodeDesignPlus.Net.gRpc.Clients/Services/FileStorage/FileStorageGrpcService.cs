namespace CodeDesignPlus.Net.gRpc.Clients.Services.FileStorage;

/// <summary>
/// Implementación de <see cref="IFileStorageGrpc"/> sobre el cliente generado de <c>FileStorage.Files</c>.
/// </summary>
/// <remarks>
/// <para>
/// No se llama <c>FileStorageService</c> porque ese nombre ya lo usa
/// <c>CodeDesignPlus.Net.File.Storage.Services.FileStorageService</c>, y ms-filestorage y sus vecinos
/// referencian los dos paquetes.
/// </para>
/// <para>
/// A diferencia de los demás clientes, no depende de <c>IUserContext</c>: lo llaman jobs de Hangfire y
/// consumidores del bus, donde no hay contexto HTTP. Ahí <c>IUserContext.AccessToken</c> devuelve
/// <see langword="null"/> y <c>Tenant</c> cae al del evento, así que la cabecera saldría como
/// <c>"Bearer "</c> vacía. Por eso no se envía <c>Authorization</c>, y <c>X-Tenant</c> se toma del propio
/// request, que es donde viaja la copropiedad dueña del archivo.
/// </para>
/// </remarks>
/// <param name="client">El cliente gRPC generado del servicio <c>FileStorage.Files</c>.</param>
public class FileStorageGrpcService(Files.FilesClient client) : IFileStorageGrpc
{
    /// <inheritdoc/>
    public async Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await client.UploadAsync(request, BuildHeaders(request), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Arma las cabeceras de la llamada a partir del request, nunca del contexto HTTP.
    /// </summary>
    private static Grpc.Core.Metadata BuildHeaders(UploadFileRequest request)
    {
        var headers = new Grpc.Core.Metadata();

        if (!string.IsNullOrWhiteSpace(request.Tenant))
            headers.Add("X-Tenant", request.Tenant);

        return headers;
    }
}
