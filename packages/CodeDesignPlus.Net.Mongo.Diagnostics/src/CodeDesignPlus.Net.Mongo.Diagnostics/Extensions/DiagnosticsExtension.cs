using CodeDesignPlus.Net.Mongo.Diagnostics.Abstractions.Options;
using CodeDesignPlus.Net.Mongo.Diagnostics.Subscriber;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;

namespace CodeDesignPlus.Net.Mongo.Diagnostics.Extensions
{
    /// <summary>
    /// Extension methods for the diagnostics
    /// </summary>
    public static class DiagnosticsExtension
    {
        /// <summary>
        /// Commands that have the collection name as a value
        /// </summary>
        private static readonly HashSet<string> Commands = new()
        {
            "aggregate",
            "count",
            "distinct",
            "mapReduce",
            "geoSearch",
            "delete",
            "find",
            "killCursors",
            "findAndModify",
            "insert",
            "update",
            "create",
            "drop",
            "createIndexes",
            "listIndexes"
        };

        /// <summary>
        /// Get the collection name from the <see cref="CommandStartedEvent"/>
        /// </summary>
        /// <param name="event">Command started event</param>
        /// <returns>Return the collection name</returns>
        public static string GetCollectionName(this CommandStartedEvent @event)
        {
            if (@event.CommandName == "getMore")
            {
                if (@event.Command.Contains("collection"))
                {
                    var collectionValue = @event.Command.GetValue("collection");

                    if (collectionValue.IsString)
                        return collectionValue.AsString;
                }
            }
            else if (Commands.Contains(@event.CommandName))
            {
                var commandValue = @event.Command.GetValue(@event.CommandName);

                if (commandValue != null && commandValue.IsString)
                    return commandValue.AsString;
            }

            return null;
        }

        /// <summary>
        /// Subscribe the <see cref="DiagnosticsActivityEventSubscriber"/> to the <see cref="ClusterBuilder"/>
        /// </summary>
        /// <param name="builder">Cluster builder</param>
        /// <param name="serviceProvider">Service provider</param>
        /// <returns>Returns the <see cref="ClusterBuilder"/> with the <see cref="DiagnosticsActivityEventSubscriber"/> subscribed</returns>
        public static ClusterBuilder SubscribeDiagnosticsActivityEventSubscriber(this ClusterBuilder builder, IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDiagnosticsOptions>>().Value;

            if (!options.Enable)
                return builder;

            var subscriber = serviceProvider.GetRequiredService<DiagnosticsActivityEventSubscriber>();

            builder.Subscribe(subscriber);

            return builder;
        }
    }
}
