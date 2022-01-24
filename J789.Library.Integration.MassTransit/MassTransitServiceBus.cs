using J789.Library.Integration.Abstraction;
using J789.Library.Integration.Abstraction.ServiceBus;
using J789.Library.Integration.MassTransit.Wrappers;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Integration.MassTransit
{
    public class MassTransitServiceBus : IServiceBus
    {
        private readonly ILogger<IServiceBus> _logger;
        private readonly IBusControl _bus;
        public MassTransitServiceBus(ILogger<IServiceBus> logger, IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
            InstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Object instance Id
        /// </summary>
        public Guid InstanceId { get; }


        public async Task PublishAsync<TIntegrationEvent>(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
        {
            try
            {
                await _bus.Publish(integrationEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to publish {IntegrationEventType} {@IntegrationEvent}", typeof(TIntegrationEvent).Name, integrationEvent);
                throw;
            }
        }

        public Task StartServiceAsync(CancellationToken cancellationToken = default)
            => _bus.StartAsync(cancellationToken);

        public Task StopServiceAsync(CancellationToken cancellationToken = default)
            => _bus.StopAsync(cancellationToken);

        public async Task<IIntegrationEventResponseContext<TResponseMessage>> RequestAsync<TResponseMessage, TRequestMessage>(TRequestMessage integrationEvent)
            where TRequestMessage : class, IIntegrationEvent
            where TResponseMessage : class, IIntegrationEvent
        {
            try
            {
                var client = _bus.CreateRequestClient<TRequestMessage>();
                var ret = await client.GetResponse<TResponseMessage>(integrationEvent);
                return new IntegrationEventResponseContext<TResponseMessage>(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RPC call between {ResponseMessageType} <--> {RequestMessageType}. {@RequestMessage}", typeof(TResponseMessage).Name, typeof(TRequestMessage).Name, integrationEvent);
                throw;
            }
        }
    }
}
