global using System;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Threading.Tasks;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using CodeDesignPlus.Net.Core.Abstractions;
global using CodeDesignPlus.Net.Core.Abstractions.Options;
global using CodeDesignPlus.Net.PubSub.Abstractions;
global using CodeDesignPlus.Net.PubSub.Extensions;
global using CodeDesignPlus.Net.RabbitMQ.Abstractions;
global using CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;
global using CodeDesignPlus.Net.RabbitMQ.Attributes;
global using CodeDesignPlus.Net.RabbitMQ.Exceptions;
global using CodeDesignPlus.Net.RabbitMQ.Services;
global using CodeDesignPlus.Net.Serializers;

global using RabbitMQ.Client;
global using RabbitMQ.Client.Events;

global using System.Collections.Concurrent;