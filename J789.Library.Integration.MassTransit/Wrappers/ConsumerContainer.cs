using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using MassTransit;
using System.Threading.Tasks;

namespace J789.Library.Integration.MassTransit.Wrappers
{
    public class ConsumerContainer<TIntegrationEvent, TIntegrationEventHandler> : IConsumer<TIntegrationEvent>
        where TIntegrationEvent : class, IIntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        private readonly TIntegrationEventHandler _integrationEventHandler;
        public ConsumerContainer(TIntegrationEventHandler integrationEventHandler)
        {
            _integrationEventHandler = integrationEventHandler;
        }

        public async Task Consume(ConsumeContext<TIntegrationEvent> context)
        {
            var intgContext = new IntegrationEventContext<TIntegrationEvent>(context);
            await _integrationEventHandler.Handle(intgContext);
        }
    }
}
