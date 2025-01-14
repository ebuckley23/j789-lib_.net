﻿using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace J789.Library.Integration.MassTransit.Wrappers
{
    public class IntegrationEventContext<TIntegrationEvent> : IIntegrationEventContext<TIntegrationEvent>
            where TIntegrationEvent : class, IIntegrationEvent
    {
        private readonly ConsumeContext<TIntegrationEvent> _consumeContext;
        public IntegrationEventContext(ConsumeContext<TIntegrationEvent> consumeContext)
        {
            _consumeContext = consumeContext ?? throw new ArgumentNullException("consumeContext cannot be null");
        }

        /// <summary>
        /// Message headers
        /// </summary>
        public IDictionary<string, object> Headers
            => _consumeContext.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value);

        /// <summary>
        /// MessageId assigned to the message when sent
        /// </summary>
        public Guid? MessageId => _consumeContext.MessageId;
        /// <summary>
        /// CorrelationId, if set, is used to track messages across multiple systems
        /// </summary>
        public Guid? CorrelationId => _consumeContext.CorrelationId;
        /// <summary>
        /// ConversationId of the message
        /// </summary>
        public Guid? ConversationId => _consumeContext.ConversationId;
        public Guid? RequestId => _consumeContext.RequestId;
        public bool TryGetMessage(out TIntegrationEvent integrationEvent)
        {
            var ret = false;
            integrationEvent = null;
            if (_consumeContext.TryGetMessage(out ConsumeContext<TIntegrationEvent> ctx))
            {
                integrationEvent = ctx.Message;
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// IntegrationEvent message
        /// </summary>
        public TIntegrationEvent IntegrationEvent => _consumeContext.Message;
        /// <summary>
        /// Reply to message sent by caller. The caller can await this and receive the response.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task ReplyAsync<TMessage>(TMessage message) where TMessage : class
        {
            SetCorrelationProperties(message);
            return _consumeContext.RespondAsync(message);
        }

        /// <summary>
        /// Reply to the message sent by caller. The caller can await this and recieve the response
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task ReplyAsync<TMessage>(object message) where TMessage : class
            => _consumeContext.RespondAsync(message);

        private void SetCorrelationProperties(object obj)
        {
            var objType = obj.GetType();
            var intgMessage = this.IntegrationEvent;
            if (objType.IsClass && typeof(IIntegrationEvent).IsAssignableFrom(objType))
            {
                //TODO: Think about doing this a different way as magic strings are fragile during a refactor
                // however I would like any value changes to these properties to be intentional, thus this is why
                // these methods exist
                if (this.CorrelationId.HasValue)
                    objType.GetMethod("SetCorrelationId").Invoke(obj, new object[] { this.CorrelationId });
                if (this.ConversationId.HasValue)
                    objType.GetMethod("SetConversationId").Invoke(obj, new object[] { this.ConversationId });
                if ((intgMessage?.ExperienceId.HasValue ?? false))
                    objType.GetMethod("SetExperienceId").Invoke(obj, new object[] { intgMessage?.ExperienceId }); 
            }
        }

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : class
        {
            SetCorrelationProperties(message);
            return _consumeContext.Publish(message);
        }

        public Task PublishAsync<TMessage>(object message) where TMessage : class
            => PublishAsync(message);
    }
}
