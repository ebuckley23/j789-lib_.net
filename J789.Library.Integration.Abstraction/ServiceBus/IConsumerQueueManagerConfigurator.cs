namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IConsumerQueueManagerConfigurator
    {
        const string NO_QUEUE_SPECIFIED = "NO_QUEUE_SPECIFIED";
        /// <summary>
        /// Subscribe to IntegrationEvent
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Integration Event to subscribe to</typeparam>
        /// <typeparam name="TIntegrationEventHandler">Event Handler to be executed when event is received from EventBus</typeparam>
        /// <param name="queueName"></param>
        void Subscribe<TIntegrationEvent, TIntegrationEventHandler>(string queueName = NO_QUEUE_SPECIFIED)
            where TIntegrationEvent : class, IIntegrationEvent;


        /// <summary>
        /// Unsubscribe to IntegrationEvent
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Integration Event to unsubscribe from</typeparam>
        /// <typeparam name="TIntegrationEventHandler">Event Handler to be executed when event is unsubscribed from</typeparam>
        /// <param name="queueName"></param>
        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>(string queueName = NO_QUEUE_SPECIFIED)
            where TIntegrationEvent : class, IIntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}
