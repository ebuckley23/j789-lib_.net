using System;
using System.Collections.Generic;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IQueueInfo
    {
        /// <summary>
        /// Name of the configured queue
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Collection of consumers for queue
        /// </summary>
        IReadOnlyCollection<ISubscriptionInfo> Subscriptions { get; }

        /// <summary>
        /// Add subscription information to queue information
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Integration Event to be handled by consumer</typeparam>
        /// <typeparam name="TIntegrationEventHandler"></typeparam>
        /// <returns></returns>
        ISubscriptionInfo AddSubscriptionInfo<TIntegrationEvent, TIntegrationEventHandler>();

        /// <summary>
        /// Add subscription information to queue information
        /// </summary>
        /// <param name="integrationEventType"></param>
        /// <param name="integrationEventHandlerType"></param>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        ISubscriptionInfo AddSubscriptionInfo(Type integrationEventType, Type integrationEventHandlerType, bool isDynamic);
        void RemoveSubscriptionInfo<TIntegrationEvent, TIntegrationEventHandler>();
        void RemoveSubscriptionInfo(Type integrationEventType, Type integrationEventHandlerType);
        void SetConnected(bool connected = true);
        bool IsConnected { get; }
    }
}
