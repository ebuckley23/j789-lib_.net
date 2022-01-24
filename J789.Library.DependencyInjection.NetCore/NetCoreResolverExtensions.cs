using J789.Library.DependencyInjection.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace J789.Library.DependencyInjection.NetCore
{
    public static class NetCoreResolverExtensions
    {
        public static IDependencyResolver ToResolver(this IServiceCollection services)
            => new NetCoreDependencyResolver(services);
    }
}
