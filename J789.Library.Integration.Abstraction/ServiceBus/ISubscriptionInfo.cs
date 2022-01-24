using System;

namespace J789.Library.Integration.Abstraction.ServiceBus
{
    public interface ISubscriptionInfo
    {
        /// <summary>
        /// The type configured to handle the equivalent EventType
        /// </summary>
        Type HandlerType { get; }

        /// <summary>
        /// The EventType to be handled by the equivalent HandlerType
        /// </summary>
        Type EventType { get; }

        /// <summary>
        /// Consumer registration status
        /// </summary>
        bool IsRegistered { get; }

        /// <summary>
        /// Set IsRegistered status
        /// </summary>
        /// <param name="isRegistered"></param>
        void SetRegistered(bool isRegistered);
    }
}
