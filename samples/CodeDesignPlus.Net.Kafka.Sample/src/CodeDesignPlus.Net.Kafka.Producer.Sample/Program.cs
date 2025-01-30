// See https://aka.ms/new-console-template for more information

using CodeDesignPlus.Net.Kafka.Extensions;
using CodeDesignPlus.Net.Kafka.Producer.Sample;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddKafka(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var pubSub = serviceProvider.GetRequiredService<IPubSub>();

var userCreatedDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com");

await pubSub.PublishAsync(userCreatedDomainEvent, CancellationToken.None);

Console.WriteLine("Message published successfully");

Console.ReadLine();