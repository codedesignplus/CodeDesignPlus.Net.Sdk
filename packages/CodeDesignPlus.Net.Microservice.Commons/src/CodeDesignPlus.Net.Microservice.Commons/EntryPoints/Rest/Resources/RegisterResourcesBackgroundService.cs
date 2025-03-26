using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Services.gRpc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using Action = CodeDesignPlus.Net.Services.gRpc.Action;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;

/// <summary>
/// Background service to register the resources of the microservice in the service registry.
/// </summary>
/// <typeparam name="TProgram">The program class of the microservice.</typeparam>
/// <param name="healthCheck">Resource health check to verify that the resources have been registered.</param>
/// <param name="options">The options of the microservice.</param>
/// <param name="logger">The logger of the microservice.</param>
/// <param name="client">The client to communicate with the service registry.</param>
public class RegisterResourcesBackgroundService<TProgram>(ResourceHealtCheck healthCheck, IOptions<CoreOptions> options, ILogger<RegisterResourcesBackgroundService<TProgram>> logger, Service.ServiceClient client) : BackgroundService
    where TProgram : class
{
    /// <summary>
    /// The resource health check to verify that the resources have been registered.
    /// </summary>
    /// <param name="stoppingToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the long-running operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            logger.LogInformation("Register Resources Background Service is stopping.");
        });

        var microservice = new Services.gRpc.Microservice()
        {
            Id = options.Value.Id.ToString(),
            Name = options.Value.AppName,
            Description = options.Value.Description
        };

        var controllers = typeof(TProgram).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ControllerBase)));

        foreach (var item in controllers)
        {
            var controller = new Services.gRpc.Controller()
            {
                Id = Guid.NewGuid().ToString(),
                Name = item.Name,
                Description = item.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
            };

            var methods = item.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                var action = new Action()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = method.Name,
                    Description = method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
                    HttpMethod = method.GetCustomAttributes().FirstOrDefault(attr => attr is HttpMethodAttribute) is HttpMethodAttribute httpMethod ? ConvertToEnum(httpMethod.HttpMethods.First()) : Services.gRpc.HttpMethod.None
                };

                controller.Actions.Add(action);
            }

            microservice.Controllers.Add(controller);
        }

        await client.CreateServiceAsync(new CreateServiceRequest()
        {
            Service = microservice
        }, cancellationToken: stoppingToken);

        logger.LogInformation("Resources registered in the service registry.");

        healthCheck.RegisterResourcesCompleted = true;
    }

    /// <summary>
    /// Convert the string value to the HttpMethod enum.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The HttpMethod enum.</returns>
    private static Services.gRpc.HttpMethod ConvertToEnum(string value)
    {
        return value switch
        {
            "GET" => Services.gRpc.HttpMethod.Get,
            "POST" => Services.gRpc.HttpMethod.Post,
            "PUT" => Services.gRpc.HttpMethod.Put,
            "DELETE" => Services.gRpc.HttpMethod.Delete,
            "PATCH" => Services.gRpc.HttpMethod.Patch,
            _ => Services.gRpc.HttpMethod.None,
        };
    }
}
