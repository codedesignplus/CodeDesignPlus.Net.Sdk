
using CodeDesignPlus.Net.RabbitMQ.Extensions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.RabbitMQ.Producer.Sample;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddRabbitMQ<Program>(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var producer = serviceProvider.GetRequiredService<IMessage>();

do
{
    var userCreatedDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com");

    await producer.PublishAsync(userCreatedDomainEvent, CancellationToken.None);

    Console.WriteLine("Message published successfully");

    await Task.Delay(1000);
} while ( true);