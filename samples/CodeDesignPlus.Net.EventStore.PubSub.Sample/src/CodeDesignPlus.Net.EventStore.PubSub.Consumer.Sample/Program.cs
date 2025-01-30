// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.EventStore.PubSub.Extensions;

Console.WriteLine("This is a sample of how to use the CodeDesignPlus.Net.EventStore.PubSub package.");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddEventStorePubSub(builder.Configuration);

var host = builder.Build();
host.Run();