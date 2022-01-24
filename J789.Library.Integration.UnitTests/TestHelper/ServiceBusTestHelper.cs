using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit;
using J789.Library.Integration.MassTransit.Wrappers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Integration.UnitTests.Fixtures
{
    class ServiceBusTestHelper : IDisposable
    {
        public readonly InMemoryTestHarness TestHarness;
        private IServiceBus _serviceBus;
        private Mock<ILogger<IServiceBus>> logger = new();
        private Mock<IBusControl> mockBusControl = new Mock<IBusControl>();

        public ServiceBusTestHelper()
        {
            TestHarness = new InMemoryTestHarness();

            mockBusControl.Setup(x => x.Publish(It.IsAny<IIntegrationEvent>(), It.IsAny<CancellationToken>()))
                .Callback(async (IIntegrationEvent obj, CancellationToken token) =>
                {
                    object o = obj;
                    await TestHarness.InputQueueSendEndpoint.Send(o);
                });
        }

        public ConsumerTestHarness<ConsumerContainer<TIntegrationEvent, TIntegrationEventHandler>> AddConsumerToTestHarness<TIntegrationEvent, TIntegrationEventHandler>(TIntegrationEventHandler handler)
            where TIntegrationEvent : class, IIntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var container = new ConsumerContainer<TIntegrationEvent, TIntegrationEventHandler>(handler);

            return TestHarness.Consumer(() => container);
        }

        public IServiceBus GetServiceBus()
        {
            _serviceBus = new MassTransitServiceBus(logger.Object, mockBusControl.Object);
            return _serviceBus;
        }

        public void Dispose()
        {
            TestHarness.Stop();
        }

        /// <summary>
        /// Ensure all testable consumers have been added before starting as consumers can not be added once bus is started
        /// </summary>
        public async Task Start()
        {
            await TestHarness.Start();
            await GetServiceBus().StartServiceAsync();
        }
    }
}
