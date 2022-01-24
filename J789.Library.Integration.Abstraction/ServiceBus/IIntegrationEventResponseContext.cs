using System;
using System.Collections.Generic;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IIntegrationEventResponseContext<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        Guid? RequestId { get; }
        Guid? MessageId { get; }
        Guid? ConversationId { get; }
        Guid? CorrelationId { get; }
        TIntegrationEvent IntegrationEvent { get; }
        IDictionary<string, object> Headers { get; }
        DateTime? SentTime { get; }
    }
}
