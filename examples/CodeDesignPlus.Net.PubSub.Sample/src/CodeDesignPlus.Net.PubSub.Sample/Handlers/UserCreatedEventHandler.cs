using System;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.PubSub.Sample.Handlers;

public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent data, CancellationToken token)
    {
        logger.LogInformation("Handling event {@data}", data);

        return Task.CompletedTask;
    }
}
