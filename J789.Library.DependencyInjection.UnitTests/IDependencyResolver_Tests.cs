using J789.Library.DependencyInjection.Lamar;
using J789.Library.DependencyInjection.NetCore;
using J789.Library.DependencyInjection.UnitTests.Fakes;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace J789.Library.DependencyInjection.UnitTests
{
    public class IDependencyResolver_Tests
    {
        [Fact]
        public void Can_Resolve_Classes_Registered_Via_Lamar()
        {
            var container = new Container(x => 
            {
                x.For<IOrderService>().Use<OrderService>();
            });
            var resolver = container.ToResolver();

            var orderService = resolver.Resolve<IOrderService>();

            Assert.IsType<OrderService>(orderService);
            Assert.Equal(nameof(LamarDependencyResolver), resolver.Name);
        }

        [Fact]
        public void Can_Resolve_Classes_Registered_Via_ServiceCollection()
        {
            var sc = new ServiceCollection();
            sc.AddTransient<IOrderService>(sp => new OrderService());

            var resolver = sc.ToResolver();

            var orderService = resolver.Resolve<IOrderService>();
            var orderService2 = resolver.Resolve<IOrderService>();

            Assert.IsType<OrderService>(orderService);
            Assert.NotEqual(orderService, orderService2);
            Assert.Equal(nameof(NetCoreDependencyResolver), resolver.Name);
        }

        [Fact]
        public void Can_Register_Singleton_Via_Lamar()
        {
            var container = new Container(sp => { });
            var r = container.ToResolver();

            r.RegisterSingleton<IOrderService, OrderService>();
            r.RegisterSingleton(typeof(IOrderService), new OrderService());
            var instance1 = r.Resolve<IOrderService>();
            var instance2 = r.Resolve<IOrderService>();

            Assert.Equal(instance1, instance2);
        }

        [Fact]
        public void Can_Register_Singleton_Via_NetCore()
        {
            var sc = new ServiceCollection();
            var r = sc.ToResolver();

            r.RegisterSingleton<IOrderService, OrderService>();
            var instance1 = r.Resolve<IOrderService>();
            var instance2 = r.Resolve<IOrderService>();

            Assert.Equal(instance1, instance2);
        }

        [Fact]
        public void Can_Get_Service_By_Type()
        {
            var netCoreResolver = new ServiceCollection().ToResolver();
            var lamarResolver = new Container(_ => { }).ToResolver();

            netCoreResolver.RegisterScoped(typeof(IOrderService), new OrderService());
            lamarResolver.RegisterScoped(typeof(IOrderService), new OrderService());

            var instance1 = netCoreResolver.Resolve(typeof(IOrderService));
            var instance2 = lamarResolver.Resolve(typeof(IOrderService));

            Assert.IsType<OrderService>(instance1);
            Assert.IsType<OrderService>(instance2);
        }

        [Fact]
        public void Can_Get_Service_By_Name()
        {
            var expectedName = "MyOrderService";
            var netCoreResolver = new ServiceCollection().ToResolver();
            netCoreResolver.RegisterSingleton(typeof(IOrderService), typeof(OrderService));
            var lamarResolver = new Container(_ =>
            {
                _.For<IOrderService>().Use<OrderService>().Named(expectedName);
            }).ToResolver();

            var instance2 = lamarResolver.Resolve<IOrderService>(expectedName);

            Assert.Throws<NotImplementedException>(() => netCoreResolver.Resolve<IOrderService>(expectedName));
            Assert.IsType<OrderService>(instance2);
        }
    }
}
