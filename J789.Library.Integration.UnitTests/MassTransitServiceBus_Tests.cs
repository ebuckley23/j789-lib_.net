using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit;
using J789.Library.Integration.MassTransit.Wrappers;
using J789.Library.Integration.UnitTests.Fakes;
using J789.Library.Integration.UnitTests.Fixtures;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Integration.UnitTests
{
    public class MassTransitServiceBus_Tests
    {
        public Mock<ILogger<IServiceBus>> logger = new();
        public Mock<IBusControl> mockBusControl = new Mock<IBusControl>();
        public MassTransitServiceBus_Tests()
        {

        }
        [Fact]
        public async Task Test1()
        {
            var harness = new InMemoryTestHarness();
            var handler = new TestIntegrationEventHandler();
            var container = new ConsumerContainer<IUserCreatedIE, TestIntegrationEventHandler>(handler);

            var consumerHarness = harness.Consumer(() => container);
            await harness.Start();

            // MassTransitServiceBus uses IBusControl.Publish method for publishing messages
            // Mock this method to use the test harness send method for testing
            mockBusControl.Setup(x => x.Publish(It.IsAny<IIntegrationEvent>(), It.IsAny<CancellationToken>()))
                .Callback(async (IIntegrationEvent obj, CancellationToken token) =>
                {
                    // box it
                    object o = obj;
                    await harness.InputQueueSendEndpoint.Send(o);
                });

            var serviceBus = new MassTransitServiceBus(logger.Object, mockBusControl.Object);
            await serviceBus.StartServiceAsync();

            try
            {
                await serviceBus.PublishAsync<IUserCreatedIE>(new UserCreatedIE { Message = nameof(Test1) });
                //await harness.InputQueueSendEndpoint.Send<IUserCreatedIE>(new UserCreatedIE { Message = nameof(Test1) });

                // did the endpoint consume the message
                Assert.True(await harness.Consumed.Any<IUserCreatedIE>());

                // did the actual consumer consume the message
                Assert.True(await consumerHarness.Consumed.Any<IUserCreatedIE>());

            }
            finally
            {
                await serviceBus.StopServiceAsync();
            }
        }

        [Fact]
        public async Task Test2()
        {
            var fixture = new ServiceBusTestHelper();
            var consumerHarness = fixture
                .AddConsumerToTestHarness<IUserCreatedIE, TestIntegrationEventHandler>(
                new TestIntegrationEventHandler());

            await fixture.Start();

            var serviceBus = fixture.GetServiceBus();

            await serviceBus.PublishAsync<IUserCreatedIE>(new UserCreatedIE { Message = nameof(Test1) });

            // did the endpoint consume the message
            Assert.True(await fixture.TestHarness.Consumed.Any<IUserCreatedIE>());

            // did the actual consumer consume the message
            Assert.True(await consumerHarness.Consumed.Any<IUserCreatedIE>());

            // the consumer published the event
            //Assert.True(await fixture.TestHarness.Published.Any<IUserCreatedResponseIE>());

            // ensure that no faults were published by the consumer
            Assert.False(await fixture.TestHarness.Published.Any<Fault<IUserCreatedIE>>());
        }
    }
}
