using J789.Library.DependencyInjection.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace J789.Library.DependencyInjection.NetCore
{
    public class NetCoreDependencyResolver : IDependencyResolver
    {
        private readonly IServiceCollection _services;
        private bool _providerHasBeenBuilt;
        private IServiceProvider _serviceProvider;

        public NetCoreDependencyResolver(IServiceCollection services)
            => _services = services;

        public string Name => nameof(NetCoreDependencyResolver);

        public IDependencyResolver RegisterScoped<TImplementation>(Type type, TImplementation implementation)
        {
            _services.AddScoped(type, sp => implementation);
            return this;
        }

        public IDependencyResolver RegisterSingleton<TType, TImplementation>() where TImplementation : class, TType
        {
            _services.AddSingleton(typeof(TType), typeof(TImplementation));
            return this;
        }

        public IDependencyResolver RegisterSingleton<TImplementation>(Type interfaceType, TImplementation implementation) where TImplementation : class
        {
            _services.AddSingleton(interfaceType, implementation);
            return this;
        }

        public TInstance Resolve<TInstance>()
        {
            if (!_providerHasBeenBuilt) BuildServiceProvider();
            return _serviceProvider.GetService<TInstance>();
        }

        public TInstance Resolve<TInstance>(string instanceName)
        {
            throw new NotImplementedException($"{nameof(NetCoreDependencyResolver)} does not support dependency resolution by name");
        }

        public object Resolve(Type instanceType)
        {
            if (!_providerHasBeenBuilt) BuildServiceProvider();
            return _serviceProvider.GetService(instanceType);
        }

        private void BuildServiceProvider()
        {
            _serviceProvider = _services.BuildServiceProvider();
            _providerHasBeenBuilt = true;
        }
    }
}
