using J789.Library.DependencyInjection.Abstraction;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace J789.Library.DependencyInjection.Lamar
{
    public class LamarDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;
        public LamarDependencyResolver(IContainer container)
            => _container = container;
        public string Name => nameof(LamarDependencyResolver);

        public IDependencyResolver RegisterScoped<TImplementation>(Type type, TImplementation implementation)
        {
            _container.Configure(sc =>
            {
                sc.AddScoped(type, sp => implementation);
            });
            return this;
        }

        public IDependencyResolver RegisterSingleton<TType, TImplementation>() where TImplementation : class, TType
        {
            _container.Configure(sc =>
            {
                sc.AddSingleton(typeof(TType), typeof(TImplementation));
            });
            return this;
        }

        public IDependencyResolver RegisterSingleton<TImplementation>(Type interfaceType, TImplementation implementation) where TImplementation : class
        { 
            _container.Configure(sc =>
             {
                 sc.AddSingleton(interfaceType, implementation);
             });
            return this;
        }

        public TInstance Resolve<TInstance>()
            => _container.GetInstance<TInstance>();

        public TInstance Resolve<TInstance>(string instanceName)
            => _container.GetInstance<TInstance>(instanceName);

        public object Resolve(Type instanceType)
            => _container.GetInstance(instanceType);
    }
}
