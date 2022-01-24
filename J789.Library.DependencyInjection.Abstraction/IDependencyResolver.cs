using System;

namespace J789.Library.DependencyInjection.Abstraction
{
    public interface IDependencyResolver
    {
        string Name { get; }

        /// <summary>
        /// Resolve a registered instance
        /// </summary>
        /// <typeparam name="TInstance">Type of instance to resolve dependency to</typeparam>
        /// <returns></returns>
        TInstance Resolve<TInstance>();

        /// <summary>
        /// Resolve a registered instance
        /// </summary>
        /// <typeparam name="TInstance">Type of instance to resolve dependency to</typeparam>
        /// <param name="instanceName">Name of instance to search for</param>
        /// <returns></returns>
        TInstance Resolve<TInstance>(string instanceName);
        object Resolve(Type instanceType);
        IDependencyResolver RegisterSingleton<TType, TImplementation>()
            where TImplementation : class, TType;
        IDependencyResolver RegisterSingleton<TImplementation>(Type interfaceType, TImplementation implementation)
            where TImplementation : class;
        IDependencyResolver RegisterScoped<TImplementation>(Type type, TImplementation implementation);
    }
}
