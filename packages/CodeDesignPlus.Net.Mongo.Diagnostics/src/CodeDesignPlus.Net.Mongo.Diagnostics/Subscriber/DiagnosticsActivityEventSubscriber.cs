namespace CodeDesignPlus.Net.Mongo.Diagnostics.Subscriber
{
    /// <summary>
    /// Subscriber for the MongoDB driver events
    /// </summary>
    public class DiagnosticsActivityEventSubscriber : IEventSubscriber
    {
        public const string ActivityName = "MongoDB.Driver.Core.Events.Command";

        private readonly MongoDiagnosticsOptions options;
        private readonly IActivityService activityService;
        private readonly ReflectionEventSubscriber subscriber;

        /// <summary>
        /// Create a new instance of <see cref="DiagnosticsActivityEventSubscriber"/>
        /// </summary>
        /// <param name="options">The options for the diagnostics</param>
        /// <param name="activityService">The service to manage the activities</param>
        public DiagnosticsActivityEventSubscriber(IOptions<MongoDiagnosticsOptions> options, IActivityService activityService)
        {
            this.options = options.Value;
            this.activityService = activityService;

            this.subscriber = new ReflectionEventSubscriber(this, bindingFlags: BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Try to get the event handler for the specified type
        /// </summary>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <param name="handler">The event handler</param></param>
        /// <returns>Returns true if the event handler was found</returns>
        public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
        {
            return this.subscriber.TryGetEventHandler(out handler);
        }

        /// <summary>
        /// Handle the <see cref="CommandStartedEvent"/>
        /// </summary>
        /// <param name="event">The event to handle</param>
        public void Handle(CommandStartedEvent @event)
        {
            var activity = this.activityService.StartActivity(ActivityName, ActivityKind.Client);

            if (activity == null)
                return;

            var collectionName = @event.GetCollectionName();

            activity.DisplayName = collectionName == null ? $"mongodb.{@event.CommandName}" : $"{collectionName}.{@event.CommandName}";

            activity.AddTag("db.system", "mongodb");
            activity.AddTag("db.connection_id", @event.ConnectionId?.ToString());
            activity.AddTag("db.name", @event.DatabaseNamespace?.DatabaseName);
            activity.AddTag("db.mongodb.collection", collectionName);
            activity.AddTag("db.operation", @event.CommandName);
            activity.AddTag("network.transport", "tcp");

            var endPoint = @event.ConnectionId?.ServerId?.EndPoint;

            switch (endPoint)
            {
                case IPEndPoint ipEndPoint:
                    activity.AddTag("network.peer.address", ipEndPoint.Address.ToString());
                    activity.AddTag("network.peer.port", ipEndPoint.Port.ToString());
                    break;
                case DnsEndPoint dnsEndPoint:
                    activity.AddTag("server.address", dnsEndPoint.Host);
                    activity.AddTag("server.port", dnsEndPoint.Port.ToString());
                    break;
            }

            if (activity.IsAllDataRequested && options.EnableCommandText)
                activity.AddTag("db.statement", @event.Command.ToString());

            this.activityService.AddActivity(@event.RequestId, activity);
        }

        /// <summary>
        /// Handle the <see cref="CommandSucceededEvent"/>
        /// </summary>
        /// <param name="event">The event to handle</param>
        public void Handle(CommandSucceededEvent @event)
        {
            if (this.activityService.RemoveActivity(@event.RequestId, out var activity))
            {
                ToogleActivity(activity, () =>
                {
                    activity.AddTag("otel.status_code", "OK");
                    activity.SetStatus(ActivityStatusCode.Ok);
                    activity.Stop();
                });
            }
        }

        /// <summary>
        /// handle the <see cref="CommandFailedEvent"/>
        /// </summary>
        /// <param name="event">The event to handle</param>
        public void Handle(CommandFailedEvent @event)
        {
            if (this.activityService.RemoveActivity(@event.RequestId, out var activity))
            {
                ToogleActivity(activity, () =>
                {
                    if (activity.IsAllDataRequested)
                    {
                        activity.AddTag("otel.status_code", "ERROR");
                        activity.AddTag("otel.status_description", @event.Failure.Message);
                        activity.AddTag("exception.type", @event.Failure.GetType().FullName);
                        activity.AddTag("exception.message", @event.Failure.Message);
                        activity.AddTag("exception.stacktrace", @event.Failure.StackTrace);
                    }

                    activity.SetStatus(ActivityStatusCode.Error);
                    activity.Stop();
                });
            }
        }

        /// <summary>
        /// Toogle the current activity
        /// </summary>
        /// <param name="activity">The activity to toogle</param>
        /// <param name="action">The action to execute</param></param>
        private static void ToogleActivity(Activity activity, Action action)
        {
            var current = Activity.Current;
            try
            {
                Activity.Current = activity;
                action();
            }
            finally
            {
                Activity.Current = current;
            }
        }
    }
}
