using System.ComponentModel.DataAnnotations;

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
    /// Gets or sets the name
    /// </summary>
    public string? PaymentUrl { get; set; } = null!;
}
