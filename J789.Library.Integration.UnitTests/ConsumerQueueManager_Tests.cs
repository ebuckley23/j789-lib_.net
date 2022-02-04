using J789.Library.DependencyInjection.NetCore;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit.QueueManager;
using J789.Library.Integration.MassTransit.Wrappers;
using J789.Library.Integration.UnitTests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace J789.Library.Integration.UnitTests
{
    public class ConsumerQueueManager_Tests
    {
        [Fact]
        public void QueueInformation_Is_Created_When_Queue_Doesnt_Exist()
        {
            var cqm = new ConsumerQueueManager();

            var queue = cqm.GetOrCreateQueue(nameof(QueueInformation_Is_Created_When_Queue_Doesnt_Exist));

            Assert.NotNull(queue);
            Assert.Equal(nameof(QueueInformation_Is_Created_When_Queue_Doesnt_Exist), queue.QueueName);
        }

        [Fact]
        public void Can_Add_And_Remove_SubscriptionInformation_To_QueueInformation()
        {
            var cqm = new ConsumerQueueManager();

            ((IConsumerQueueManagerConfigurator)cqm).Subscribe<UserCreatedIE, TestIntegrationEventHandler>(nameof(Can_Add_And_Remove_SubscriptionInformation_To_QueueInformation));

            var qs = cqm.GetQueuesBySubscriptionEventType<UserCreatedIE>();

            Assert.Contains(qs, x => x.QueueName == nameof(Can_Add_And_Remove_SubscriptionInformation_To_QueueInformation));

            var q = qs.Single(x => x.QueueName == nameof(Can_Add_And_Remove_SubscriptionInformation_To_QueueInformation));

            Assert.Contains(q.Subscriptions, x => x.EventType == typeof(UserCreatedIE) && x.HandlerType == typeof(TestIntegrationEventHandler));

            ((IConsumerQueueManagerConfigurator)cqm).Unsubscribe<UserCreatedIE, TestIntegrationEventHandler>(nameof(Can_Add_And_Remove_SubscriptionInformation_To_QueueInformation));

            var qs1 = cqm.GetQueuesBySubscriptionEventType<UserCreatedIE>();

            Assert.Empty(qs1);
        }

        [Fact]
        public void Can_Get_All_Queues()
        {
            var cqm = new ConsumerQueueManager();
            ((IConsumerQueueManagerConfigurator)cqm).Subscribe<UserCreatedResponseIE, TestIntegrationEventHandler>(nameof(Can_Get_All_Queues));

            var qs = cqm.GetAllQueues();

            Assert.NotEmpty(qs);
            Assert.Contains(qs, x => x.QueueName == nameof(Can_Get_All_Queues));
        }

        [Fact]
        public void Can_Resolve_Consumer()
        {
            var sc = new ServiceCollection();
            var cqm = new ConsumerQueueManager();
            var di = new NetCoreDependencyResolver(sc);

            di.RegisterScoped(typeof(TestIntegrationEventHandler), new TestIntegrationEventHandler());

            var si = new SubscriptionInfo(typeof(TestIntegrationEventHandler), typeof(IUserCreatedIE));

            var instance = cqm.GetConsumer(di, si);

            Assert.Equal(typeof(ConsumerContainer<IUserCreatedIE, TestIntegrationEventHandler>), instance.GetType());
        }
    }
}
