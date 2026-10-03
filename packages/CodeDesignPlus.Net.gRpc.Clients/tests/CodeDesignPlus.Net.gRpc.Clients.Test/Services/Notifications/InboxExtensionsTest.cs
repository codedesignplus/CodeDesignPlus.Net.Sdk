using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Notifications;

/// <summary>
/// Cubre lo que el emisor no puede equivocarse al mandar un aviso durable.
/// </summary>
/// <remarks>
/// Regla 12 seccion 5: los tres serializadores del repo producen PascalCase por defecto y el frontend deja
/// de leer el payload sin fallar y sin log. Las extensiones son la unica puerta publica justamente para que
/// nadie tenga que acordarse de las <c>JsonSerializerOptions</c>.
/// </remarks>
public class InboxExtensionsTest
{
    private sealed record Payload(Guid DocumentId, long AmountDue);

    [Fact]
    public async Task TheJsonPayloadIsSerializedInCamelCase()
    {
        var captured = await CaptureAsync(grpc => grpc.NotifyUsersAsync(
            [Guid.NewGuid()], "invoice.issued", "Cuenta de cobro emitida", "Su cuota ya esta disponible.",
            module: "invoicing", aggregateId: Guid.NewGuid().ToString(),
            payload: new Payload(Guid.NewGuid(), 15_000_00L),
            tenant: Guid.NewGuid(), sentBy: Guid.Empty));

        Assert.Contains("\"documentId\"", captured.JsonPayload);
        Assert.Contains("\"amountDue\"", captured.JsonPayload);
        Assert.DoesNotContain("\"DocumentId\"", captured.JsonPayload);
    }

    [Fact]
    public async Task TheNotificationCarriesAClientGeneratedId()
    {
        // Es la clave de idempotencia frente a las reentregas del bus: si lo generara el servidor, cada
        // reentrega crearia un aviso nuevo y la campana mostraria el mismo hecho tres veces.
        var captured = await CaptureAsync(grpc => grpc.NotifyTenantAsync(
            "admin.broadcast", "Corte de agua", "Manana de 8 a 12.",
            module: null, aggregateId: null, payload: new { }, tenant: Guid.NewGuid(), sentBy: Guid.Empty));

        Assert.True(Guid.TryParse(captured.Id, out var id) && id != Guid.Empty);
    }

    [Fact]
    public async Task ANoticeForUsersCarriesEachUserInItsAudience()
    {
        var uno = Guid.NewGuid();
        var otro = Guid.NewGuid();

        var captured = await CaptureAsync(grpc => grpc.NotifyUsersAsync(
            [uno, otro], "invoice.issued", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));

        Assert.Equal(AudienceKind.User, captured.Audience.Kind);
        Assert.Equal([uno.ToString(), otro.ToString()], captured.Audience.Values);
    }

    [Fact]
    public async Task ANoticeForRolesCarriesTheRoleNames()
    {
        // Los nombres de rol viajan sin resolver a proposito: quien emite no conoce los usuarios detras del
        // rol, y resolverlo aqui seria una llamada cruzada en el path de escritura.
        var captured = await CaptureAsync(grpc => grpc.NotifyRolesAsync(
            ["Administrador", "Contador"], "pqrs.created", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));

        Assert.Equal(AudienceKind.Role, captured.Audience.Kind);
        Assert.Equal(["Administrador", "Contador"], captured.Audience.Values);
    }

    [Fact]
    public async Task ATenantWideNoticeCarriesNoValues()
    {
        var captured = await CaptureAsync(grpc => grpc.NotifyTenantAsync(
            "admin.broadcast", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));

        Assert.Equal(AudienceKind.Tenant, captured.Audience.Kind);
        Assert.Empty(captured.Audience.Values);
    }

    [Fact]
    public async Task ANoticeWithoutAModuleCarriesNoResource()
    {
        // Un aviso general no lleva a ninguna parte, y la bandeja lo pinta sin enlace. Mandar un Resource
        // vacio en su lugar produciria un enlace roto en la campana.
        var captured = await CaptureAsync(grpc => grpc.NotifyTenantAsync(
            "admin.broadcast", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));

        Assert.Null(captured.Resource);
    }

    [Fact]
    public async Task ANoticeStampsWhenItHappened()
    {
        var captured = await CaptureAsync(grpc => grpc.NotifyTenantAsync(
            "admin.broadcast", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));

        Assert.NotNull(captured.OccurredAt);
        Assert.True(captured.OccurredAt.ToDateTimeOffset() > DateTimeOffset.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task AGivenNotificationIdTravelsAsTheId()
    {
        // Una reentrega del bus vuelve a llamar con el mismo id derivado del evento: tiene que llegar tal cual a
        // la bandeja, o la bandeja no puede reconocer el repetido (pendings/250).
        var eventId = Guid.NewGuid();
        var expected = NotificationIds.For(eventId, "governance.appointed");

        var first = await CaptureAsync(grpc => grpc.NotifyUsersAsync(
            [Guid.NewGuid()], "governance.appointed", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty,
            notificationId: NotificationIds.For(eventId, "governance.appointed")));
        var redelivery = await CaptureAsync(grpc => grpc.NotifyUsersAsync(
            [Guid.NewGuid()], "governance.appointed", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty,
            notificationId: NotificationIds.For(eventId, "governance.appointed")));

        Assert.Equal(expected.ToString(), first.Id);
        Assert.Equal(first.Id, redelivery.Id);
    }

    [Fact]
    public async Task WithoutANotificationIdEachCallGetsItsOwn()
    {
        var first = await CaptureAsync(grpc => grpc.NotifyTenantAsync(
            "admin.broadcast", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));
        var second = await CaptureAsync(grpc => grpc.NotifyTenantAsync(
            "admin.broadcast", "t", "b", null, null, new { }, Guid.NewGuid(), Guid.Empty));

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void TheDerivedIdIsStableAndDistinguishesSourceAndKind()
    {
        var source = Guid.Parse("38baffab-507e-4d35-965a-62c153f10fda");

        var id = NotificationIds.For(source, "governance.appointed");

        Assert.Equal(id, NotificationIds.For(source, "governance.appointed"));
        Assert.NotEqual(id, NotificationIds.For(source, "governance.removed"));
        Assert.NotEqual(id, NotificationIds.For(Guid.NewGuid(), "governance.appointed"));
        Assert.Equal('5', id.ToString()[14]);
    }

    private static async Task<NotificationRequest> CaptureAsync(Func<IInboxGrpc, Task> send)
    {
        NotificationRequest? captured = null;

        var grpc = new Mock<IInboxGrpc>();
        grpc.Setup(x => x.NotifyAsync(It.IsAny<NotificationRequest>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationRequest, CancellationToken>((r, _) => captured = r)
            .Returns(Task.CompletedTask);

        await send(grpc.Object);

        Assert.NotNull(captured);

        return captured!;
    }
}
