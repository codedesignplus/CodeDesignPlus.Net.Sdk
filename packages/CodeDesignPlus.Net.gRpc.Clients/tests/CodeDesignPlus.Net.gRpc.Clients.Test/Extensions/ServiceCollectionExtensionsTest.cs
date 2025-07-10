using CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;
using CodeDesignPlus.Net.gRpc.Clients.Extensions;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;
using CodeDesignPlus.Net.gRpc.Clients.Services.Users;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddGrpcClients_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddGrpcClients(services, Mock.Of<IConfiguration>()));
    }

    [Fact]
    public void AddGrpcClients_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddGrpcClients(services, configuration));
    }

    [Fact]
    public void AddGrpcClients_RegistersPaymentClient_WhenPaymentOptionIsSet()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{GrpcClientsOptions.Section}:Payment"] = "http://localhost:5001"
            })
            .Build();

        // Act
        services.AddGrpcClients(config);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.Contains(services, d => d.ServiceType.Name.Contains("PaymentClient"));
        Assert.Contains(services, d => d.ServiceType == typeof(IPaymentGrpc) && d.ImplementationType == typeof(PaymentService));

        var paymentService = serviceProvider.GetService<Payment.PaymentClient>();

        Assert.NotNull(paymentService);
    }

    [Fact]
    public void AddGrpcClients_RegistersUserClient_WhenUserOptionIsSet()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{GrpcClientsOptions.Section}:User"] = "http://localhost:5002"
            })
            .Build();

        // Act
        services.AddGrpcClients(config);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.Contains(services, d => d.ServiceType.Name.Contains("UsersClient"));
        Assert.Contains(services, d => d.ServiceType == typeof(IUserGrpc) && d.ImplementationType == typeof(UserService));

        var userService = serviceProvider.GetService<Clients.Services.User.Users.UsersClient>();

        Assert.NotNull(userService);
    }

    [Fact]
    public void AddGrpcClients_RegistersTenantClient_WhenTenantOptionIsSet()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{GrpcClientsOptions.Section}:Tenant"] = "http://localhost:5003"
            })
            .Build();

        // Act
        services.AddGrpcClients(config);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.Contains(services, d => d.ServiceType.Name.Contains("TenantClient"));
        Assert.Contains(services, d => d.ServiceType == typeof(ITenantGrpc) && d.ImplementationType == typeof(TenantService));

        var tenantService = serviceProvider.GetService<Clients.Services.Tenant.Tenant.TenantClient>();
        Assert.NotNull(tenantService);
    }

    [Fact]
    public void AddGrpcClients_RegistersNothing_WhenNoOptionsAreSet()
    {
        // Arrange
        var services = new ServiceCollection();

        var settings = new
        {
            GrpcClients = new
            { 
                Info = "No gRPC clients configured",
            }
        };

        var json = JsonSerializer.Serialize(settings);

        var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var config = new ConfigurationBuilder()
            .AddJsonStream(jsonStream)
            .Build();

        // Act
        services.AddGrpcClients(config);

        // Assert
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IPaymentGrpc));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IUserGrpc));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ITenantGrpc));
    }
}
