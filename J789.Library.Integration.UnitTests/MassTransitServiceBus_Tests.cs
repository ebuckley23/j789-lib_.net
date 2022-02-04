using J789.Library.DependencyInjection.Lamar;
using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit;
using J789.Library.Integration.MassTransit.Wrappers;
using J789.Library.Integration.UnitTests.Fakes;
using J789.Library.Integration.UnitTests.Fixtures;
using Lamar;
using MassTransit;
using MassTransit.Context;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
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
        
        [Fact]
        public void Can_Configure_MTServiceBus_With_NetCore_DI()
        {
            var sc = new ServiceCollection();
            sc.AddLogging();

            sc.AddScoped<TestIntegrationEventHandler>();
            sc.AddMassTransitServiceBus(c =>
            {
                c.Subscribe<IUserCreatedIE, TestIntegrationEventHandler>(nameof(Can_Configure_MTServiceBus_With_NetCore_DI));
            },
            "",
            true,
            MTBusIntegration.InMemory);

            var sp = sc.BuildServiceProvider();
            var bus = sp.GetService<IServiceBus>();

            Assert.NotNull(bus);
        }

        [Fact]
        public void Can_Configure_MTServiceBus_With_Lamar_DI()
        {
            var registry = new ServiceRegistry();
            registry.AddLogging();

            registry.Scan(s =>
            {
                s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.FullName.Contains("Integration.UnitTests"));
                s.WithDefaultConventions();
                //s.SingleImplementationsOfInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IIntegrationEventHandler<>), ServiceLifetime.Scoped);
            });

            var container = new Container(registry);

            registry.AddMassTransitServiceBus(c =>
            {
                c.Subscribe<IUserCreatedIE, TestIntegrationEventHandler>(nameof(Can_Configure_MTServiceBus_With_Lamar_DI));
            },
            "",
            true,
            MTBusIntegration.InMemory,
            container.ToResolver());

            var bus = container.GetService<IServiceBus>();
            Assert.NotNull(bus);
        }

        [Fact]
        public void Configuration_Throws_InvalidOperation_If_IntegrationEventHandler_Not_Registered_Via_DI()
        {
            var sc = new ServiceCollection();
            sc.AddLogging();

            Assert.Throws<InvalidOperationException>(() => sc.AddMassTransitServiceBus(c =>
            {
                c.Subscribe<IUserCreatedIE, TestIntegrationEventHandler>(nameof(Configuration_Throws_InvalidOperation_If_IntegrationEventHandler_Not_Registered_Via_DI));
            },
            "",
            true,
            MTBusIntegration.InMemory));
        }

        [Fact]
        public async Task Can_Convert_MT_ConsumeContext_To_IntegrationEventContext()
        {
            var evt = new UserCreatedIE { Message = nameof(Can_Convert_MT_ConsumeContext_To_IntegrationEventContext) };
            var convId = Guid.NewGuid();
            var corId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var headerkey1 = "testheader1";
            var headerkey2 = "testheader2";

            IntegrationEventContext<IUserCreatedIE> testContext = null;
            UserCreatedResponseIE replyTestMsg = null;
            UserCreatedResponseIE publishTestMsg = null;

            var mockConsumeContext = new Mock<ConsumeContext<IUserCreatedIE>>();
            var headers = new DictionarySendHeaders(
                new Dictionary<string, object> 
                {
                    { headerkey1, "value1" },
                    { headerkey2, "value2" }
                });
            mockConsumeContext.SetupGet(x => x.Message).Returns(evt);
            mockConsumeContext.SetupGet(x => x.ConversationId).Returns(convId);
            mockConsumeContext.SetupGet(x => x.CorrelationId).Returns(corId);
            mockConsumeContext.SetupGet(x => x.Headers).Returns(headers);
            mockConsumeContext.SetupGet(x => x.MessageId).Returns(messageId);
            mockConsumeContext.SetupGet(x => x.RequestId).Returns(requestId);
            mockConsumeContext.Setup(x => x.RespondAsync(It.IsAny<UserCreatedResponseIE>()))
                .Callback((dynamic r) =>
                {
                    replyTestMsg = r;
                });
            mockConsumeContext.Setup(x => x.Publish(It.IsAny<UserCreatedResponseIE>(), default))
                .Callback((dynamic p, CancellationToken t) =>
                {
                    publishTestMsg = p;
                });

            var mockHandler = new Mock<ITestIntegrationEventHandler>();
            mockHandler.Setup(x => x.Handle(It.IsAny<IntegrationEventContext<IUserCreatedIE>>()))
                .Callback(async (dynamic iec) =>
                {
                    testContext = iec;
                    await ((IntegrationEventContext<IUserCreatedIE>)iec).PublishAsync(new UserCreatedResponseIE { Message = "PUBLISHED" });
                    await ((IntegrationEventContext<IUserCreatedIE>)iec).ReplyAsync(new UserCreatedResponseIE { Message = "REPLIED"});
                });

            var cc = new ConsumerContainer<IUserCreatedIE, ITestIntegrationEventHandler>(mockHandler.Object);
            await cc.Consume(mockConsumeContext.Object);

            // initial context message should contain properties
            Assert.Equal(messageId, testContext.MessageId);
            Assert.Equal(requestId, testContext.RequestId);
            Assert.Equal(convId, testContext.ConversationId);
            Assert.Equal(corId, testContext.CorrelationId);
            Assert.True(testContext.Headers.ContainsKey(headerkey1));
            Assert.True(testContext.Headers.ContainsKey(headerkey2));
            Assert.Equal(evt.Message, testContext.IntegrationEvent.Message);

            // response message should transfer the following properties
            Assert.Equal(corId, replyTestMsg.CorrelationId); 
            Assert.Equal(convId, replyTestMsg.ConversationId);
            Assert.Equal("REPLIED", replyTestMsg.Message);

            // publish message should transfer the following properties
            Assert.Equal(corId, publishTestMsg.CorrelationId);
            Assert.Equal(convId, publishTestMsg.ConversationId);
            Assert.Equal("PUBLISHED", publishTestMsg.Message);
        }
    }
}
