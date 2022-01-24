using J789.Library.DependencyInjection.Abstraction;
using Lamar;

namespace J789.Library.DependencyInjection.Lamar
{
    public static class LamarDependencyResolverExtensions
    {
        public static IDependencyResolver ToResolver(this IContainer container)
            => new LamarDependencyResolver(container);
    }
}
