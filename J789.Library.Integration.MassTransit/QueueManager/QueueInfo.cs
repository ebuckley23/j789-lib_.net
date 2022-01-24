using J789.Library.Integration.Abstraction.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Integration.MassTransit.QueueManager
{
    public class QueueInfo : IQueueInfo
    {
        private List<ISubscriptionInfo> _items = new List<ISubscriptionInfo>();
        public QueueInfo(string queueName)
        {
            QueueName = queueName;
        }

        public string QueueName { get; }

        public IReadOnlyCollection<ISubscriptionInfo> Subscriptions
            => _items.AsReadOnly();

        public bool IsConnected { get; private set; }

        public ISubscriptionInfo AddSubscriptionInfo<TIntegrationEvent, TIntegrationEventHandler>()
            => this.AddSubscriptionInfo(typeof(TIntegrationEvent), typeof(TIntegrationEventHandler), false);

        public ISubscriptionInfo AddSubscriptionInfo(Type integrationEventType, Type integrationEventHandlerType, bool isDynamic)
        {
            if(HandlerExists(integrationEventType, integrationEventHandlerType))
            {
                throw new ApplicationException($"{QueueName} already contains a consumer of type {integrationEventHandlerType}");
            }

            var info = new SubscriptionInfo(integrationEventHandlerType, integrationEventType);
            _items.Add(info);
            return info;
        }

        public void RemoveSubscriptionInfo<TIntegrationEvent, TIntegrationEventHandler>()
            => RemoveSubscriptionInfo(typeof(TIntegrationEvent), typeof(TIntegrationEventHandler));

        public void RemoveSubscriptionInfo(Type integrationEventType, Type integrationEventHandlerType)
        {
            var subscription = _items.Single(x => x.HandlerType == integrationEventHandlerType && x.EventType == integrationEventType);
            _items.Remove(subscription);
        }

        public void SetConnected(bool connected = true)
            => IsConnected = connected;

        private bool HandlerExists(Type eventType, Type consumerType)
            => _items.Any(x => x.HandlerType == consumerType && x.EventType == eventType);
    }
}
