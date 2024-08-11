global using System;
global using System.Text;
global using System.Threading.Tasks;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using CodeDesignPlus.Net.Core.Abstractions;
global using CodeDesignPlus.Net.EventStore.Abstractions;
global using CodeDesignPlus.Net.EventStore.Extensions;
global using CodeDesignPlus.Net.EventStore.PubSub.Abstractions;
global using CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;
global using CodeDesignPlus.Net.EventStore.PubSub.Exceptions;
global using CodeDesignPlus.Net.EventStore.PubSub.Services;
global using CodeDesignPlus.Net.PubSub.Abstractions;
global using CodeDesignPlus.Net.PubSub.Extensions;
global using CodeDesignPlus.Net.Serializers;

global using EventStore.ClientAPI;
global using EventStore.ClientAPI.SystemData;

