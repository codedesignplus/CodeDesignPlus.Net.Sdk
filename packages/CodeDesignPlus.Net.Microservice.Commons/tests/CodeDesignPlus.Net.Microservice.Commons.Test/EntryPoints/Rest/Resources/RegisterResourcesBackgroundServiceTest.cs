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
            mockClient.Object
        );

        // Act
        await backgroundService.StartAsync(CancellationToken.None);

        // Assert
        Assert.True(healtCheck.RegisterResourcesCompleted);
    }
}
