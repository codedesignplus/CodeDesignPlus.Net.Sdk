global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Linq;
global using System.Reflection;
global using System.Security.Cryptography;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

global using Azure.Identity;
global using Azure.Messaging.ServiceBus;
global using Azure.Messaging.ServiceBus.Administration;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using CodeDesignPlus.Net.Core.Abstractions;
global using CodeDesignPlus.Net.Core.Abstractions.Options;
global using CodeDesignPlus.Net.PubSub.Abstractions;
global using CodeDesignPlus.Net.PubSub.Abstractions.Attributes;
global using CodeDesignPlus.Net.PubSub.Extensions;
global using CodeDesignPlus.Net.PubSub.Services;
global using CodeDesignPlus.Net.Serializers;
global using CodeDesignPlus.Net.ServiceBus.Abstractions;
global using CodeDesignPlus.Net.ServiceBus.Abstractions.Options;
global using CodeDesignPlus.Net.ServiceBus.Exceptions;
global using CodeDesignPlus.Net.ServiceBus.Services;
