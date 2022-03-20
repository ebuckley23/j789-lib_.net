using J789.Library.Integration.Abstraction.ServiceBus;
using System.Threading.Tasks;

namespace J789.Library.Integration.UnitTests.Fakes
{
    public class TestIntegrationEventHandler : ITestIntegrationEventHandler
    {
        public async Task Handle(IIntegrationEventContext<IUserCreatedIE> context)
        {
            await context.PublishAsync(new UserCreatedResponseIE { Message = "Received" });
        }
    }

    /// <summary>
    /// Added for mocking capabilities
    /// </summary>
    public interface ITestIntegrationEventHandler : IIntegrationEventHandler<IUserCreatedIE>
    {

    }
}
