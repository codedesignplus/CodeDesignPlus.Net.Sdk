using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Notifications;

/// <summary>
/// Cubre lo que el emisor no puede equivocarse al empujar un mensaje efimero.
/// </summary>
public class LiveChannelExtensionsTest
{
    private sealed record Payload(int ProcessedUnits, int TotalUnits);

    [Fact]
    public async Task TheJsonPayloadIsSerializedInCamelCase()
    {
        LiveUserPush? captured = null;

        var grpc = new Mock<ILiveChannelGrpc>();
        grpc.Setup(x => x.PushToUserAsync(It.IsAny<LiveUserPush>(), It.IsAny<CancellationToken>()))
            .Callback<LiveUserPush, CancellationToken>((r, _) => captured = r)
            .Returns(Task.CompletedTask);

        await grpc.Object.PushToUserAsync(
            Guid.NewGuid(), NotificationKinds.Live.ChargeGenerationProgress, new Payload(37, 200), Guid.NewGuid());

        Assert.Contains("\"processedUnits\"", captured!.JsonPayload);
        Assert.DoesNotContain("\"ProcessedUnits\"", captured.JsonPayload);
    }

    [Fact]
    public async Task TheGroupNameTravelsUnqualified()
    {
        // El servidor le antepone "Tenant:{tenant}:". Calificarlo tambien aqui daria un grupo con el
        // prefijo dos veces, al que nadie esta suscrito, y cuyo fallo es el silencio: el push se escribe,
        // el acuse dice success, y nadie recibe nada.
        LiveGroupPush? captured = null;

        var grpc = new Mock<ILiveChannelGrpc>();
        grpc.Setup(x => x.PushToGroupAsync(It.IsAny<LiveGroupPush>(), It.IsAny<CancellationToken>()))
            .Callback<LiveGroupPush, CancellationToken>((r, _) => captured = r)
            .Returns(Task.CompletedTask);

        var tenant = Guid.NewGuid();

        await grpc.Object.PushToGroupAsync("assembly-42", "FloorGranted", new { }, tenant);

        Assert.Equal("assembly-42", captured!.GroupName);
        Assert.Equal(tenant.ToString(), captured.Tenant);
    }
}
