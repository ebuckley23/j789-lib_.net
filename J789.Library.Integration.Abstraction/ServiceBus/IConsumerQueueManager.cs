using J789.Library.DependencyInjection.Abstraction;
using System.Collections.Generic;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IConsumerQueueManager : IConsumerQueueManagerConfigurator
    {
        /// <summary>
        /// Returns or creates and returns a queue configuration with the specified queue name
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        IQueueInfo GetOrCreateQueue(string queueName);

        /// <summary>
        /// Returns a queue configuration for the specified queue name
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        IQueueInfo Get(string queueName);

        /// <summary>
        /// Returns a collection of queue configurations created
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IQueueInfo> GetAllQueues();

        /// <summary>
        /// Returns a collection of queue configurations by integration event type
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <returns></returns>
        IEnumerable<IQueueInfo> GetQueuesBySubscriptionEventType<TIntegrationEvent>()
            where TIntegrationEvent : IIntegrationEvent;

        /// <summary>
        /// Returns consumer
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="subscriptionInfo"></param>
        /// <returns></returns>
        object GetConsumer(IDependencyResolver resolver, ISubscriptionInfo subscriptionInfo);
    }
}
