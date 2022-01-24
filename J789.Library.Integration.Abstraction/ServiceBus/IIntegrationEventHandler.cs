using System.Threading.Tasks;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : class, IIntegrationEvent
    {
        /// <summary>
        /// When a message of the specified TIntegrationEvent type is received, this handle method is called.
        /// </summary>
        /// <param name="context">IIntegrationEventContext that provides correlationIds and the ability to reply to messages if desired</param>
        /// <returns></returns>
        Task Handle(IIntegrationEventContext<TIntegrationEvent> context);
    }

    public interface IIntegrationEventHandler { }
}
