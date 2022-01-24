using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IIntegrationEventContext<out TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        IDictionary<string, object> Headers { get; }
        TIntegrationEvent IntegrationEvent { get; }
        Task ReplyAsync<TMessage>(TMessage message) where TMessage : class;
        Task ReplyAsync<TMessage>(object message) where TMessage : class;
        Task PublishAsync<TMessage>(TMessage message) where TMessage : class;
        Task PublishAsync<TMessage>(object message) where TMessage : class;
        /// <summary>
        /// MessageId assigned to the message when sent
        /// </summary>
        Guid? MessageId { get; }
        /// <summary>
        /// CorrelationId, if set, is used to track messages across multiple systems
        /// </summary>
        Guid? CorrelationId { get; }
        /// <summary>
        /// ConversationId of the message
        /// </summary>
        Guid? ConversationId { get; }
        Guid? RequestId { get; }
    }
}
