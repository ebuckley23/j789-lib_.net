using J789.Library.DependencyInjection.Abstraction;
using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Integration.MassTransit.QueueManager
{
    public class ConsumerQueueManager : IConsumerQueueManager
    {
        private List<IQueueInfo> _items = new List<IQueueInfo>();
        public IQueueInfo Get(string queueName)
            => _items.SingleOrDefault(x => x.QueueName.Equals(queueName));

        public IReadOnlyCollection<IQueueInfo> GetAllQueues()
            => _items.AsReadOnly();

        public object GetConsumer(IDependencyResolver resolver, ISubscriptionInfo subscriptionInfo)
        {
            Type[] genericArgs = { subscriptionInfo.EventType, subscriptionInfo.HandlerType };
            var makeme = typeof(ConsumerContainer<,>).MakeGenericType(genericArgs);
            var instance = resolver.Resolve(subscriptionInfo.HandlerType);
            if (instance == null)
            {
                throw new InvalidOperationException($"Unable to resolve {subscriptionInfo.HandlerType.Name}. Ensure type has been registered with {resolver.Name}");
            }
            return Activator.CreateInstance(makeme, instance);
        }

        public IQueueInfo GetOrCreateQueue(string queueName)
        {
            var q = Get(queueName);
            if (q != null) return q;

            var ret = new QueueInfo(queueName);
            _items.Add(ret);
            return ret;
        }

        public IEnumerable<IQueueInfo> GetQueuesBySubscriptionEventType<TIntegrationEvent>() where TIntegrationEvent : IIntegrationEvent
        {
            foreach (var q in _items)
            {
                if (q.Subscriptions.Any(x => x.EventType == typeof(TIntegrationEvent)))
                {
                    yield return q;
                }
            }
        }

        void IConsumerQueueManagerConfigurator.Subscribe<TIntegrationEvent, TIntegrationEventHandler>(string queueName)
        {
            var q = GetOrCreateQueue(queueName);
            if(q != null)
            {
                q.AddSubscriptionInfo<TIntegrationEvent, TIntegrationEventHandler>();
            }
        }

        void IConsumerQueueManagerConfigurator.Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>(string queueName)
        {
            var q = Get(queueName);
            if(q != null)
            {
                q.RemoveSubscriptionInfo<TIntegrationEvent, TIntegrationEventHandler>();
            }
        }
    }
}
