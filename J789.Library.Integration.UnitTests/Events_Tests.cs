using J789.Library.Integration.Abstraction.Enums;
using System;
using Xunit;

namespace J789.Library.Integration.UnitTests
{
    public class Events_Tests
    {
        [Fact]
        public void Can_Create_New_IntegrationEventLog_From_IntegrationEvent()
        {
            var corId = Guid.NewGuid();
            var expId = Guid.NewGuid();
            var convId = Guid.NewGuid();

            var intgEvent = new IntegrationEvent();
            intgEvent.SetCorrelationId(corId);
            intgEvent.SetExperienceId(expId);
            intgEvent.SetConversationId(convId);

            var intgEventLog = new IntegrationEventLog(intgEvent);
            Assert.Null(intgEventLog.IntegrationEvent);
            intgEventLog.DeserializeJsonContent(intgEvent.GetType());
            Assert.NotNull(intgEventLog.IntegrationEvent);
            Assert.Equal(intgEvent.Id, intgEventLog.IntegrationEvent.Id);
            Assert.Equal(intgEvent.GetType().FullName, intgEventLog.EventTypeName);
            Assert.Equal(nameof(IntegrationEvent), intgEventLog.EventTypeShortName);
            Assert.Equal(EventStateEnum.NotPublished, intgEventLog.State);
            Assert.Equal(0, intgEventLog.SendCount);
            Assert.Equal(corId, intgEventLog.IntegrationEvent.CorrelationId);
            Assert.Equal(expId, intgEventLog.IntegrationEvent.ExperienceId);
            Assert.Equal(convId, intgEventLog.IntegrationEvent.ConversationId);
        }
    }
}
