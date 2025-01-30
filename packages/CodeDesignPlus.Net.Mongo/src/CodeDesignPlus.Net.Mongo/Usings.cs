﻿global using System;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using CodeDesignPlus.Net.Core.Abstractions;
global using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
global using CodeDesignPlus.Net.Criteria.Extensions;
global using CodeDesignPlus.Net.Mongo.Abstractions;
global using CodeDesignPlus.Net.Mongo.Abstractions.Operations;
global using CodeDesignPlus.Net.Mongo.Abstractions.Options;
global using CodeDesignPlus.Net.Mongo.Converter;
global using CodeDesignPlus.Net.Mongo.Diagnostics.Extensions;
global using CodeDesignPlus.Net.Mongo.Repository;
global using CodeDesignPlus.Net.Security.Abstractions;
global using MongoDB.Bson;
global using MongoDB.Bson.Serialization;
global using MongoDB.Driver;
global using MBS = MongoDB.Bson.Serialization;
global using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
global using NodaTime;