using J789.Library.Integration.Abstraction.ServiceBus;
using System;

namespace J789.Library.Integration.MassTransit.QueueManager
{
    public class SubscriptionInfo : ISubscriptionInfo
    {
        public SubscriptionInfo(Type handlerType, Type eventType)
        {
            HandlerType = handlerType;
            EventType = eventType;
        }

        public Type HandlerType { get; }

        public Type EventType { get; }

        public bool IsRegistered { get; private set; }

        public void SetRegistered(bool isRegistered)
            => IsRegistered = isRegistered;
    }
}
