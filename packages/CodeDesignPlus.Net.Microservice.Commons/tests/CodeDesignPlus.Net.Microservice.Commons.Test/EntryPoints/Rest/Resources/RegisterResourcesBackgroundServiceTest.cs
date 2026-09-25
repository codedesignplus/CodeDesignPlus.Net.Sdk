using System;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;
using CodeDesignPlus.Net.Microservice.Commons.Test.Helpers;
using CodeDesignPlus.Net.Services.gRpc;
using Grpc.Core;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Resources;

public class RegisterResourcesBackgroundServiceTest
{
    private readonly CoreOptions coreOptions = new()
    {
        Id = Guid.NewGuid(),
        AppName = "TestApp",
        ApiDocumentationBaseUrl = "https://api.testapp.com",
        Business = "CodeDesignPlus",
        Version = "1.0.0",
        TypeEntryPoint = "rest",
        Description = "Test application for unit testing",
        Contact = new()
        {
            Name = "Test User",
            Email = "test.user@testapp.com"
        }
    };


    [Fact]
    public async Task ExecuteAsync_RegisterControllers_Success()
    {
        // Arrange
        var healtCheck = new ResourceHealtCheck();
        var loggerMock = new Mock<ILogger<RegisterResourcesBackgroundService<Startup>>>();

        var mockCall = CallHelpers.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty());

        var mockClient = new Mock<CodeDesignPlus.Net.Services.gRpc.Service.ServiceClient>();

        mockClient
            .Setup(m => m.CreateServiceAsync(It.IsAny<CreateServiceRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<CreateServiceRequest, Metadata, DateTime?, CancellationToken>((request, metadata, deadline, token) =>
            {
                var fakeController = request.Service.Controllers.FirstOrDefault(x => x.Name == "FakeController");
                Assert.NotNull(fakeController);
                Assert.NotNull(fakeController.Id);
                Assert.Equal("This is a fake controller used for testing purposes.", fakeController.Description);
                Assert.Equal("FakeController", fakeController.Name);

                var actionsFakeController = fakeController.Actions;

                Assert.NotEmpty(actionsFakeController);
                Assert.Contains(actionsFakeController, a => a.Name == "Get" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Get && a.Description == "Get method for the fake items.");
                Assert.Contains(actionsFakeController, a => a.Name == "Post" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Post && a.Description == "This method is used to create a new fake item.");
                Assert.Contains(actionsFakeController, a => a.Name == "Put" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Put && a.Description == "This method is used to update an existing fake item.");
                Assert.Contains(actionsFakeController, a => a.Name == "Delete" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Delete && a.Description == "This method is used to delete an existing fake item.");
                Assert.Contains(actionsFakeController, a => a.Name == "Patch" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Patch && a.Description == "This method is used to partially update an existing fake item.");

                var fakeControllerWithNoDescription = request.Service.Controllers.FirstOrDefault(x => x.Name == "FakeControllerWithNoDescription");

                Assert.NotNull(fakeControllerWithNoDescription);
                Assert.NotNull(fakeControllerWithNoDescription.Id);
                Assert.Equal("FakeControllerWithNoDescription", fakeControllerWithNoDescription.Name);
                Assert.Empty(fakeControllerWithNoDescription.Description);

                var actionsFakeControllerWithNoDescription = fakeControllerWithNoDescription.Actions;

                Assert.NotEmpty(actionsFakeControllerWithNoDescription);
                Assert.Contains(actionsFakeControllerWithNoDescription, a => a.Name == "Get" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Get && string.IsNullOrEmpty(a.Description));
                Assert.Contains(actionsFakeControllerWithNoDescription, a => a.Name == "Post" && a.HttpMethod == CodeDesignPlus.Net.Services.gRpc.HttpMethod.Post && string.IsNullOrEmpty(a.Description));
            })
            .Returns(mockCall);

        var backgroundService = new RegisterResourcesBackgroundService<Startup>(
            healtCheck,
            Microsoft.Extensions.Options.Options.Create(this.coreOptions),
            loggerMock.Object,
            mockClient.Object,
            this.resourcesOptions
        );

        // Act
        await backgroundService.StartAsync(CancellationToken.None);

        // Assert
        Assert.True(healtCheck.RegisterResourcesCompleted);
    }

    private readonly IOptions<ResourcesOptions> resourcesOptions = Microsoft.Extensions.Options.Options.Create(new ResourcesOptions
    {
        Enable = true,
        Server = new Uri("http://localhost:5001"),
        RetryInitialDelay = TimeSpan.FromMilliseconds(10),
        RetryMaxDelay = TimeSpan.FromMilliseconds(20)
    });

    private RegisterResourcesBackgroundService<Startup> Service(ResourceHealtCheck healthCheck, Mock<Service.ServiceClient> client, Mock<ILogger<RegisterResourcesBackgroundService<Startup>>> logger = null)
        => new(healthCheck, Microsoft.Extensions.Options.Options.Create(this.coreOptions), (logger ?? new Mock<ILogger<RegisterResourcesBackgroundService<Startup>>>()).Object, client.Object, this.resourcesOptions);

    private static async Task WaitUntil(Func<bool> condition)
    {
        for (var i = 0; i < 200 && !condition(); i++)
            await Task.Delay(10);
    }

    // Si el registro falla, el micro no se cae: reintenta, y el health check queda no listo mientras tanto
    // (pendings/019). Antes la excepcion salia de ExecuteAsync y el host se detenia.

    [Fact]
    public async Task ExecuteAsync_FallaYLuegoResponde_ReintentaYQuedaListo()
    {
        var healthCheck = new ResourceHealtCheck();
        var client = new Mock<Service.ServiceClient>();
        client.SetupSequence(m => m.CreateServiceAsync(It.IsAny<CreateServiceRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(CallHelpers.CreateAsyncUnaryCall<Google.Protobuf.WellKnownTypes.Empty>(StatusCode.FailedPrecondition))
            .Returns(CallHelpers.CreateAsyncUnaryCall<Google.Protobuf.WellKnownTypes.Empty>(StatusCode.Unavailable))
            .Returns(CallHelpers.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty()));
        var service = Service(healthCheck, client);

        await service.StartAsync(CancellationToken.None);
        await WaitUntil(() => healthCheck.RegisterResourcesCompleted);

        Assert.True(healthCheck.RegisterResourcesCompleted);
        client.Verify(m => m.CreateServiceAsync(It.IsAny<CreateServiceRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        await service.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_FallaSiempre_NoLanzaYElHealthCheckNoQuedaListo()
    {
        var healthCheck = new ResourceHealtCheck();
        var client = new Mock<Service.ServiceClient>();
        client.Setup(m => m.CreateServiceAsync(It.IsAny<CreateServiceRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(() => CallHelpers.CreateAsyncUnaryCall<Google.Protobuf.WellKnownTypes.Empty>(StatusCode.FailedPrecondition));
        var logger = new Mock<ILogger<RegisterResourcesBackgroundService<Startup>>>();
        var service = Service(healthCheck, client, logger);

        await service.StartAsync(CancellationToken.None);
        await Task.Delay(150);
        await service.StopAsync(CancellationToken.None);

        Assert.False(healthCheck.RegisterResourcesCompleted);
        Assert.NotNull(service.ExecuteTask);
        Assert.True(service.ExecuteTask.IsCompletedSuccessfully, "el servicio en segundo plano no debe terminar con excepcion: detendria el host");
        client.Verify(m => m.CreateServiceAsync(It.IsAny<CreateServiceRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
        logger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<RpcException>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
    }
}
