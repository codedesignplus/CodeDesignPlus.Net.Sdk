﻿using Moq;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using CodeDesignPlus.Net.xUnit.Helpers.OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using System.Net;

namespace CodeDesignPlus.Net.Observability.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddObservability_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(null, null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddObservability_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(configuration: null, environment: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddObservability_EnvironmentIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var options = xUnit.Helpers.ConfigurationUtil.GetConfiguration(Test.Helpers.ConfigurationUtil.ObservabilityOptions);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(options, null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'environment')", exception.Message);
    }

    [Fact]
    public void AddObservability_SectionNotExist_ObservabilityException()
    {
        // Arrange
        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new object() { });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ObservabilityException>(() => serviceCollection.AddObservability(configuration, environment));

        // Assert
        Assert.Equal($"The section {ObservabilityOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddObservability_AddServices_Success()
    {
        // Arrange
        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = Test.Helpers.ConfigurationUtil.ObservabilityOptions
        });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environment);

        // Assert
        Assert.NotEmpty(serviceCollection);
    }

    [Fact]
    public void AddObservability_DisableMetricsAndTracing_Success()
    {
        // Arrange
        var options = Test.Helpers.ConfigurationUtil.ObservabilityOptions;
        options.Metrics.Enable = false;
        options.Trace.Enable = false;

        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = options
        });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environment);

        // Assert
        Assert.NotEmpty(serviceCollection);
        Assert.Equal(20, serviceCollection.Count);
    }

    [Fact]
    public void AddObservability_OnlyMetrics_CheckNumberServices()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = true,
            ServerOtel = new Uri("http://localhost:4317"),
            Metrics = new Metrics()
            {
                Enable = true,
                AspNetCore = true
            },
            Trace = new Trace()
            {
                Enable = false,
                AspNetCore = true,
                CodeDesignPlusSdk = true,
                Redis = true,
                Kafka = true,
                SqlClient = true,
                GrpcClient = true
            }
        };

        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = options
        });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environment);

        // Assert
        Assert.NotEmpty(serviceCollection);
        Assert.Equal(36, serviceCollection.Count);
    }

    [Fact]
    public void AddObservability_OnlyTrace_CheckNumberServices()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = true,
            ServerOtel = new Uri("http://localhost:4317"),
            Metrics = new Metrics()
            {
                Enable = false
            },
            Trace = new Trace()
            {
                Enable = true,
                AspNetCore = true,
                CodeDesignPlusSdk = true,
                Redis = true,
                Kafka = true,
                SqlClient = true,
                GrpcClient = true
            }
        };

        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = options
        });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environment);

        // Assert
        Assert.NotEmpty(serviceCollection);
        Assert.Equal(49, serviceCollection.Count);
    }
}
