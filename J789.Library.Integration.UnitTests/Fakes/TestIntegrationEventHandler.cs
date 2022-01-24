using J789.Library.Integration.Abstraction.ServiceBus;
using System.Threading.Tasks;

namespace J789.Library.Integration.UnitTests.Fakes
{
    class TestIntegrationEventHandler : IIntegrationEventHandler<IUserCreatedIE>
    {
        public async Task Handle(IIntegrationEventContext<IUserCreatedIE> context)
        {
            await context.PublishAsync(new UserCreatedResponseIE { Message = "Received" });
        }
    }
}
