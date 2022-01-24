using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Integration.MassTransit.Wrappers
{
    class IntegrationEventResponseContext<TIntegrationEvent> : IIntegrationEventResponseContext<TIntegrationEvent>
        where TIntegrationEvent : class, IIntegrationEvent
    {
        private readonly Response<TIntegrationEvent> _response;
        public IntegrationEventResponseContext(Response<TIntegrationEvent> response)
        {
            _response = response;
        }

        public Guid? RequestId => _response.RequestId;

        public Guid? MessageId => _response.MessageId;

        public Guid? ConversationId => _response.ConversationId;

        public Guid? CorrelationId => _response.CorrelationId;

        public TIntegrationEvent IntegrationEvent => _response.Message;

        public IDictionary<string, object> Headers
            => _response.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value);

        public DateTime? SentTime => _response.SentTime;
    }
}
