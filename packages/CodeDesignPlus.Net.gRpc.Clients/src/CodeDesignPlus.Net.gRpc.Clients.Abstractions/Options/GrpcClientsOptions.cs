namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;

/// <summary>
/// Options to setting of the gRpc.Clients
/// </summary>
public class GrpcClientsOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "GrpcClients";

    /// <summary>
    /// Gets or sets the URL for the Payment gRPC service.
    /// </summary>
    public string Payment { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the User gRPC service.
    /// </summary>
    public string User { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the Tenant gRPC service.
    /// </summary>
    public string Tenant { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the Notification gRPC service.
    /// </summary>
    public string Notification { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the Location gRPC service.
    /// </summary>
    public string Location { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the License gRPC service.
    /// </summary>
    public string License { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the Module gRPC service.
    /// </summary>
    public string Module { get; set; } = null!;
    /// <summary>
    /// Gets or sets the URL for the Email gRPC service.
    /// </summary>
    public string Email { get; set; } = null!;
}
