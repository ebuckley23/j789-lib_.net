using System;

namespace J789.Library.Integration.Abstraction
{
    public interface IIntegrationEvent
    {
        /// <summary>
        /// Integration EventId as a GUID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Integration Creation Date as UTC time
        /// </summary>
        DateTimeOffset CreationDate { get; }

        /// <summary>
        /// CorrelationId used for correlating events across multiple systems
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// ConversationId to represent a request used for correlating a request
        /// across multiple systems.
        /// It's possible for conversation and correlationId to be the same
        /// </summary>
        Guid ConversationId { get; }

        /// <summary>
        /// ExperienceId meant to represent the entire experience. The same experienceId
        /// can be used with different conversation/correlation Ids
        /// Ex. When a user logs in or first accesses an application, then they initiate
        /// the "experience". The user could take multiple actions during the experience that
        /// would result in multiple conversations (requests) but the same experience
        /// </summary>
        Guid? ExperienceId { get; }
    }
}
