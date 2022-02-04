using J789.Library.Integration.Abstraction;
using Newtonsoft.Json;
using System;

namespace J789.Library.Integration
{
    public class IntegrationEvent : IIntegrationEvent
    {
        /// <summary>
        /// Initializes new integration event with Guid and Creation data
        /// </summary>
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CorrelationId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes new integration event with Guid and Creation data
        /// </summary>
        /// <param name="id">Unique Id of integration event</param>
        /// <param name="createDate">Event creation data</param>
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
            CorrelationId = Guid.NewGuid();
        }
        /// <summary>
        /// Integration EventId as a GUID
        /// </summary>
        [JsonProperty]
        public Guid Id { get; private set; }

        /// <summary>
        /// Integration Creation Date as UTC time
        /// </summary>
        [JsonProperty]
        public DateTimeOffset CreationDate { get; private set; }

        /// <summary>
        /// CorrelationId used for correlating events across multiple systems
        /// </summary>
        [JsonProperty]
        public Guid CorrelationId { get; private set; }

        /// <summary>
        /// ConversationId to represent a request used for correlating a request
        /// across multiple systems.
        /// It's possible for conversation and correlationId to be the same
        /// </summary>
        [JsonProperty]
        public Guid ConversationId { get; private set; }

        /// <summary>
        /// ExperienceId meant to represent the entire experience. The same experienceId
        /// can be used with different conversation/correlation Ids
        /// Ex. When a user logs in or first accesses an application, then they initiate
        /// the "experience". The user could take multiple actions during the experience that
        /// would result in multiple conversations (requests) but the same experience
        /// </summary>
        [JsonProperty]
        public Guid? ExperienceId { get; private set; }
        public void SetExperienceId(Guid experienceId) => ExperienceId = experienceId;
        public void SetConversationId(Guid conversationId) => ConversationId = conversationId;
        public void SetCorrelationId(Guid correlationId) => CorrelationId = correlationId;
    }
}
