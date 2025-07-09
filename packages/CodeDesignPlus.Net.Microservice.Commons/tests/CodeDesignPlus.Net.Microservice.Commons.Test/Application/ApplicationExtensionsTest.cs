using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Microservice.Commons.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Application;

public class ApplicationExtensionsTest
{
    private readonly CoreOptions coreOptions = new ()
    {
        Id = Guid.NewGuid(),
        AppName = "TestApp",
        ApiDocumentationBaseUrl = "https://api.testapp.com",
        Business = "CodeDesignPlus",
        Version = "1.0.0",
        TypeEntryPoint = "rest",
        Description = "Test application for unit testing",
        Contact = new ()
        {
            Name = "Test User",
            Email = "test.user@testapp.com"
        }
    };

    [Fact]
    public void UsePath_WithPathBase_CallsUsePathBase()
    {
        // Arrange
        var pathBase = "/api";
        this.coreOptions.PathBase = pathBase;
        var optionsMock = new Mock<IOptions<CoreOptions>>();
        optionsMock.Setup(o => o.Value).Returns(coreOptions);

        var services = new ServiceCollection();
        services.AddSingleton(optionsMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        var appMock = new Mock<IApplicationBuilder>();
        appMock.SetupGet(a => a.ApplicationServices).Returns(serviceProvider);
        appMock.SetupGet(x => x.Properties)
            .Returns(new Dictionary<string, object?>());

        // Act
        appMock.Object.UsePath();

        // Assert
        appMock.Verify(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
    }

    [Fact]
    public void UsePath_WithoutPathBase_DoesNotCallUsePathBase()
    {
        // Arrange
        this.coreOptions.PathBase = null;
        var optionsMock = new Mock<IOptions<CoreOptions>>();
        optionsMock.Setup(o => o.Value).Returns(coreOptions);

        var services = new ServiceCollection();
        services.AddSingleton(optionsMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        var appMock = new Mock<IApplicationBuilder>();
        appMock.SetupGet(a => a.ApplicationServices).Returns(serviceProvider);
        appMock.SetupGet(x => x.Properties)
            .Returns(new Dictionary<string, object?>());

        // Act
        appMock.Object.UsePath();

        // Assert
        appMock.Verify(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Never);
    }

    [Fact]
    public void UsePath_WithEmptyPathBase_DoesNotCallUsePathBase()
    {
        // Arrange
        this.coreOptions.PathBase = "";
        var optionsMock = new Mock<IOptions<CoreOptions>>();
        optionsMock.Setup(o => o.Value).Returns(coreOptions);

        var services = new ServiceCollection();
        services.AddSingleton(optionsMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        var appMock = new Mock<IApplicationBuilder>();
        appMock.SetupGet(a => a.ApplicationServices).Returns(serviceProvider);
        appMock.SetupGet(x => x.Properties)
            .Returns(new Dictionary<string, object?>());

        // Act
        appMock.Object.UsePath();

        // Assert
        appMock.Verify(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Never);
    }
}