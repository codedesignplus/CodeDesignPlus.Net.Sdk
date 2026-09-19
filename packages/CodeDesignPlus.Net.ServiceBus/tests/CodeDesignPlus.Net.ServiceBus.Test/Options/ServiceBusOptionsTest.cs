using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.ServiceBus.Test.Options;

public class ServiceBusOptionsTest
{
    private static List<ValidationResult> Validate(ServiceBusOptions options)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(options, new ValidationContext(options), results, true);

        return results;
    }

    [Fact]
    public void Section_Value_IsServiceBus()
    {
        Assert.Equal("ServiceBus", ServiceBusOptions.Section);
    }

    [Fact]
    public void Validate_NotEnable_ReturnsNoErrors()
    {
        // Un transporte apagado no tiene por que estar configurado: es lo que permite desplegar el paquete sin
        // obligar a todos los entornos a declarar un namespace.
        var options = new ServiceBusOptions { Enable = false };

        Assert.Empty(Validate(options));
    }

    [Fact]
    public void Validate_EnableWithoutNamespaceOrConnectionString_ReturnsError()
    {
        var options = new ServiceBusOptions { Enable = true };

        var results = Validate(options);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ServiceBusOptions.FullyQualifiedNamespace)));
    }

    [Fact]
    public void Validate_EnableWithConnectionStringOnly_ReturnsNoErrors()
    {
        var options = new ServiceBusOptions
        {
            Enable = true,
            ConnectionString = "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
        };

        Assert.Empty(Validate(options));
    }

    [Fact]
    public void Validate_EnableWithNamespaceOnly_ReturnsNoErrors()
    {
        var options = new ServiceBusOptions { Enable = true, FullyQualifiedNamespace = "contoso.servicebus.windows.net" };

        Assert.Empty(Validate(options));
    }

    [Fact]
    public void Validate_RetryIntervalGreaterThanMax_ReturnsError()
    {
        var options = new ServiceBusOptions
        {
            Enable = true,
            FullyQualifiedNamespace = "contoso.servicebus.windows.net",
            RetryIntervalMs = 30000,
            MaxRetryIntervalMs = 10000
        };

        var results = Validate(options);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ServiceBusOptions.RetryIntervalMs)));
    }

    [Fact]
    public void Validate_MaxRetryIntervalExceedsLockRenewal_ReturnsError()
    {
        // La espera del backoff corre con el bloqueo retenido. Si supera la renovacion automatica, el broker
        // reentrega el mensaje a otro consumidor mientras este todavia espera.
        var options = new ServiceBusOptions
        {
            Enable = true,
            FullyQualifiedNamespace = "contoso.servicebus.windows.net",
            MaxRetryIntervalMs = 120000,
            MaxAutoLockRenewalMinutes = 2
        };

        var results = Validate(options);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ServiceBusOptions.MaxRetryIntervalMs)));
    }

    [Fact]
    public void Validate_DefaultValues_AreConsistent()
    {
        var options = new ServiceBusOptions { Enable = true, FullyQualifiedNamespace = "contoso.servicebus.windows.net" };

        Assert.Empty(Validate(options));

        // Paridad con las 158 colas desplegadas hoy en RabbitMQ: x-delivery-limit = 11 y x-message-ttl = 48 h.
        Assert.Equal(10, options.MaxRetry);
        Assert.Equal(48, options.MessageTimeToLiveHours);
        Assert.Equal(0, options.PrefetchCount);
        Assert.True(options.AutoProvisionEntities);
        Assert.True(options.RegisterHealthCheck);
    }

    [Fact]
    public void Validate_LockDurationAboveServiceBusMaximum_ReturnsError()
    {
        var options = new ServiceBusOptions
        {
            Enable = true,
            FullyQualifiedNamespace = "contoso.servicebus.windows.net",
            LockDurationSeconds = 301
        };

        var results = Validate(options);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(ServiceBusOptions.LockDurationSeconds)));
    }
}
