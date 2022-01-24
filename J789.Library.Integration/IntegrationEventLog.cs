using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace J789.Library.Integration
{
    public class IntegrationEventLog : IIntegrationEventLog
    {
        public IntegrationEventLog(IIntegrationEvent intgEvent)
        {
            EventId = intgEvent.Id;
            CreationDate = intgEvent.CreationDate;
            EventTypeName = intgEvent.GetType().FullName;
            Content = JsonConvert.SerializeObject(intgEvent);
            State = EventStateEnum.NotPublished;
            SendCount = 0;
        }
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public EventStateEnum State { get; set; }
        public int SendCount { get; set; }
        public DateTimeOffset CreationDate { get; private set; }
        public string Content { get; private set; }
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        [NotMapped]
        public IIntegrationEvent IntegrationEvent { get; private set; }

        public IIntegrationEventLog DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(Content, type) as IntegrationEvent;
            return this;
        }
    }
}
