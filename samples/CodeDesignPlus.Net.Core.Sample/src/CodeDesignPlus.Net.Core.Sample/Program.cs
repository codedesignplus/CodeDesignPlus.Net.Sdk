// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Core.Sample.Resources.DomainEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("This is a sample of how to use the DomainEventResolverService.");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton(configuration);
serviceCollection.AddCore(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var domainEventResolverService = serviceProvider.GetRequiredService<IDomainEventResolver>();

var type = domainEventResolverService.GetDomainEventType<OrderCreatedDomainEvent>();

Console.WriteLine(type.FullName);

Console.WriteLine("Press any key to exit...");
