using System.Threading;
using System.Threading.Tasks;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IServiceBus
    {
        /// <summary>
        /// Publish Integration Event to the EventBus
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <param name="integrationEvent">Integration Event to publish</param>
        /// <returns></returns>
        Task PublishAsync<TIntegrationEvent>(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
            where TIntegrationEvent : IIntegrationEvent;

        /// <summary>
        /// Send a request to handler listening to TIntegrationEvent. API is ideally used in a RPC scenario
        /// </summary>
        /// <typeparam name="TResponseMessage">The expected response type</typeparam>
        /// <typeparam name="TRequestMessage">The TIntegration to be handled</typeparam>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        Task<IIntegrationEventResponseContext<TResponseMessage>> RequestAsync<TResponseMessage, TRequestMessage>(TRequestMessage integrationEvent)
            where TRequestMessage : class, IIntegrationEvent
            where TResponseMessage : class, IIntegrationEvent;

        /// <summary>
        /// Start the ServiceBus. This is not likely needed to be called in scenarios outside of tests
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartServiceAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the ServiceBus. This is not likely needed to be called in scenarios outside of tests
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopServiceAsync(CancellationToken cancellationToken = default);
    }
}
