using J789.Library.DependencyInjection.Abstraction;
using J789.Library.DependencyInjection.NetCore;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit.QueueManager;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Integration.MassTransit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceBus AddMassTransitServiceBus(
            this IServiceCollection services, 
            Action<IConsumerQueueManagerConfigurator> config,
            string hostUri,
            bool isTls,
            MTBusIntegration mTBusIntegration = MTBusIntegration.RabbitMQ,
            IDependencyResolver resolver = null)
        {
            var containerResolver = resolver ?? services.ToResolver();

            var qManager = new ConsumerQueueManager();
            config(qManager);
            var qs = qManager.GetAllQueues();
            var allSubscriptions = qs.SelectMany(x => x.Subscriptions);

            services.AddMassTransit(masstransit =>
            {
                foreach(var subscription in allSubscriptions)
                {
                    var consumer = qManager.GetConsumer(containerResolver, subscription);
                    masstransit.AddConsumer(consumer.GetType());
                }

                masstransit.AddBus(context =>
                {
                    switch (mTBusIntegration)
                    {
                        case MTBusIntegration.RabbitMQ: return ConfigureRabbitMq(isTls ? hostUri.Replace("amqp://", "amqps://") : hostUri, qs, qManager, containerResolver);
                        default: return ConfigureInMemory(qs, qManager, containerResolver);
                    }
                });
            });

            var servicebus = BuildServiceBus(services.BuildServiceProvider());

            // TODO: figure out a way to get around this dependency
            services.AddSingleton(servicebus);
            containerResolver.RegisterSingleton(typeof(IServiceBus), servicebus);

            return containerResolver.Resolve<IServiceBus>();
        }

        private static void ConfigureQueuesOnBus(IBusFactoryConfigurator cfg, IEnumerable<IQueueInfo> queues, IConsumerQueueManager queueManager, IDependencyResolver dependencyResolver)
        {
            foreach (var q in queues)
            {
                if (q.IsConnected) continue;

                cfg.ReceiveEndpoint(q.QueueName, endpoint =>
                {
                    foreach (var subscription in q.Subscriptions)
                    {
                        var consumer = queueManager.GetConsumer(dependencyResolver, subscription);
                        var type = consumer.GetType();
                        endpoint.Consumer(type, t => dependencyResolver.Resolve(t));
                        subscription.SetRegistered(true);
                    }
                });

                q.SetConnected();
            }
        }
        private static IBusControl ConfigureInMemory(IEnumerable<IQueueInfo> queues, IConsumerQueueManager queueManager, IDependencyResolver dependencyResolver)
        {
            var ret = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                //cfg.ConfigureJsonDeserializer(s =>
                //{
                //    s.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //    return s;
                //});

                ConfigureQueuesOnBus(cfg, queues, queueManager, dependencyResolver);
            });

            return ret;
        }
        private static IBusControl ConfigureRabbitMq(string hostUri, IEnumerable<IQueueInfo> queues, IConsumerQueueManager queueManager, IDependencyResolver dependencyResolver) 
        {
            var ret = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ConfigureJsonDeserializer(s =>
                {
                    s.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    return s;
                });
                cfg.Durable = true;
                cfg.PurgeOnStartup = false;
                cfg.AutoDelete = false;
                cfg.Host(new Uri(hostUri), _ =>
                {

                });

                ConfigureQueuesOnBus(cfg, queues, queueManager, dependencyResolver);
            });
            return ret;
        }

        private static IServiceBus BuildServiceBus(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IServiceBus>>();
            var busControl = serviceProvider.GetRequiredService<IBusControl>();
            var bus = new MassTransitServiceBus(logger, busControl);
            return bus;
        }
    }

    public enum MTBusIntegration
    {
        InMemory,
        RabbitMQ
    }
}
