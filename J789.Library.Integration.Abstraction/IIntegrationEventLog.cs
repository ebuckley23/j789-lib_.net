using J789.Library.Integration.Abstraction.Enums;
using System;

namespace J789.Library.Integration.Abstraction
{
    public interface IIntegrationEventLog
    {
        /// <summary>
        /// Content of the Integration Event that occurred
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Date log was created
        /// </summary>
        DateTimeOffset CreationDate { get; }

        /// <summary>
        /// Id of the Integration Event that occurred
        /// 
        /// Useful for correlating events across different microservices
        /// </summary>
        Guid EventId { get; }
        string EventTypeName { get; }
        string EventTypeShortName { get; }

        /// <summary>
        /// Deserialized Integration Event
        /// </summary>
        IIntegrationEvent IntegrationEvent { get; }

        /// <summary>
        /// Number of times the Integration Event attempted to be sent
        /// </summary>
        int SendCount { get; set; }

        /// <summary>
        /// Current state of the Integration Event send status
        /// </summary>
        EventStateEnum State { get; set; }

        IIntegrationEventLog DeserializeJsonContent(Type type);
    }
}
