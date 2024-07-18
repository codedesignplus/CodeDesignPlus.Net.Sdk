﻿global using System;
global using System.Collections.Concurrent;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using CodeDesignPlus.Net.Core.Abstractions;
global using CodeDesignPlus.Net.Core.Abstractions.Options;
global using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
global using CodeDesignPlus.Net.Event.Sourcing.Abstractions.Options;
global using CodeDesignPlus.Net.EventStore.Abstractions;
global using CodeDesignPlus.Net.EventStore.Abstractions.Options;
global using CodeDesignPlus.Net.EventStore.Exceptions;
global using CodeDesignPlus.Net.EventStore.Serializer;
global using CodeDesignPlus.Net.EventStore.Services;
global using CodeDesignPlus.Net.Serializers;
global using ES = EventStore.ClientAPI;

