// using CodeDesignPlus.Net.Core.Abstractions.Options;
// using CodeDesignPlus.Net.Security.gRpc;
// using Grpc.Core;
// using Grpc.Net.Client;
// using Moq;
// using S = CodeDesignPlus.Net.Security.Services;

// namespace CodeDesignPlus.Net.Security.Test.Services;

// public class RbacTest
// {
//     private readonly Mock<ILogger<S.Rbac>> loggerMock;
//     private readonly Mock<IOptions<SecurityOptions>> securityOptionsMock;
//     private readonly Mock<IOptions<CoreOptions>> coreOptionsMock;
//     private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
//     private readonly Mock<gRpc.Rbac.RbacClient> rbacClientMock;
//     private readonly S.Rbac rbacService;

//     public RbacTest()
//     {
//         loggerMock = new Mock<ILogger<S.Rbac>>();
//         securityOptionsMock = new Mock<IOptions<SecurityOptions>>();
//         coreOptionsMock = new Mock<IOptions<CoreOptions>>();
//         httpClientFactoryMock = new Mock<IHttpClientFactory>();
//         rbacClientMock = new Mock<gRpc.Rbac.RbacClient>();

//         securityOptionsMock.Setup(x => x.Value).Returns(new SecurityOptions
//         {
//             ServerRbac = new Uri("http://localhost")
//         });
//         coreOptionsMock.Setup(x => x.Value).Returns(OptionsUtil.CoreOptions);

//         var httpClient = new HttpClient();
//         httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

//         var channel = GrpcChannel.ForAddress("http://localhost");
//         rbacService = new S.Rbac(loggerMock.Object, securityOptionsMock.Object, coreOptionsMock.Object, httpClientFactoryMock.Object);
//     }

//     [Fact]
//     public async Task LoadRbacAsync_ShouldLoadResources()
//     {
//         // Arrange
//         var response = new GetRbacResponse
//         {
//             Resources = { new RbacResource { Controller = "TestController", Action = "TestAction", Method = gRpc.HttpMethod.GET, Role = "Admin" } }
//         };

//         rbacClientMock
//             .Setup(x => x.GetRbacAsync(It.IsAny<GetRbacRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
//             .Returns(response);

//         // Act
//         await rbacService.LoadRbacAsync(CancellationToken.None);

//         // Assert
//         Assert.Single(rbacService.Resources);
//     }

//     [Fact]
//     public async Task IsAuthorizedAsync_ShouldReturnTrue_WhenUserIsAuthorized()
//     {
//         // Arrange
//         var roles = new[] { "Admin" };
//         await rbacService.LoadRbacAsync(CancellationToken.None);

//         // Act
//         var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "GET", roles);

//         // Assert
//         Assert.True(result);
//     }

//     [Fact]
//     public async Task IsAuthorizedAsync_ShouldReturnFalse_WhenUserIsNotAuthorized()
//     {
//         // Arrange
//         var roles = new[] { "User" };
//         await rbacService.LoadRbacAsync(CancellationToken.None);

//         // Act
//         var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "GET", roles);

//         // Assert
//         Assert.False(result);
//     }
// }
