using CodeDesignPlus.Net.gRpc.Clients.Services.FileStorage;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Cliente del servicio gRPC de ms-filestorage para guardar archivos que se generan en el backend.
/// </summary>
/// <remarks>
/// <para>
/// Es para archivos que nacen en un microservicio —un PDF que arma un job de Hangfire, un adjunto que
/// llega por un consumidor del bus—. Un archivo que nace en el navegador <b>no</b> pasa por aquí: se sube
/// por REST a <c>/api/FileStorage/Upload</c>.
/// </para>
/// <para>
/// Quien llama no suele tener usuario ni JWT, así que la copropiedad y el usuario viajan <b>dentro</b> del
/// request (<see cref="UploadFileRequest.Tenant"/> y <see cref="UploadFileRequest.UploadedBy"/>) y no se
/// toman del contexto HTTP. El <see cref="UploadFileRequest.Id"/> lo genera quien sube: es el que guarda
/// su agregado.
/// </para>
/// </remarks>
public interface IFileStorageGrpc
{
    /// <summary>
    /// Guarda el archivo en ms-filestorage con su registro, igual que si se subiera por REST.
    /// </summary>
    /// <param name="request">El archivo, con su id, copropiedad, usuario, target y nombre original.</param>
    /// <param name="cancellationToken">Token de cancelación que se propaga a la llamada gRPC.</param>
    /// <returns>El id, el target y el nombre con el que quedó guardado.</returns>
    Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Registra en ms-filestorage un archivo que otro sistema dejó en un contenedor de la misma cuenta de almacenamiento
    /// (por ejemplo, la grabación de una llamada): ms-filestorage lo copia en el servidor a su carpeta, crea el registro y
    /// borra el origen.
    /// </summary>
    /// <remarks>
    /// El contenido no viaja en el mensaje, así que no le aplica el tope de tamaño de <see cref="UploadAsync"/>. Repetirla
    /// con el mismo <see cref="ImportFileRequest.Id"/> devuelve el archivo ya registrado, así que se puede reintentar.
    /// </remarks>
    /// <param name="request">El archivo, con su id, copropiedad, usuario, target, nombre y de dónde se toma.</param>
    /// <param name="cancellationToken">Token de cancelación que se propaga a la llamada gRPC.</param>
    /// <returns>El id, el target y el nombre con el que quedó guardado.</returns>
    Task<UploadFileResponse> ImportAsync(ImportFileRequest request, CancellationToken cancellationToken);
}
