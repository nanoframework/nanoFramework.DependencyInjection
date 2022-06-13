using System;

using nanoFramework.TestFramework;
using nanoFramework.DependencyInjection.UnitTests.Fakes;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void ServicesRegisteredWithImplementationTypeForTransientServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service1 = serviceProvider.GetService(typeof(IFakeService));
            var service2 = serviceProvider.GetService(typeof(IFakeService));

            Assert.IsType(typeof(FakeService), service1.GetType());
            Assert.IsType(typeof(FakeService), service2.GetType());
            Assert.NotSame(service1, service2);
        }

        //[TestMethod]
        //public void ServicesRegisteredWithImplementationTypeForStructTransientServices()
        //{
        //    var serviceProvider = new ServiceCollection()
        //        .AddTransient(typeof(IStructFakeService), typeof(StructFakeService))
        //        .BuildServiceProvider();

        //    var service1 = serviceProvider.GetService(typeof(IStructFakeService));
        //    var service2 = serviceProvider.GetService(typeof(IStructFakeService));

        //    Assert.IsType(typeof(StructFakeService), service1.GetType());
        //    Assert.IsType(typeof(StructFakeService), service2.GetType());
        //    Assert.NotSame(service1, service2);
        //}

        [TestMethod]
        public void ServicesRegisteredWithNoInterfaceTypeForTransientServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(FakeService))
                .BuildServiceProvider();

            var service1 = serviceProvider.GetService(typeof(FakeService));
            var service2 = serviceProvider.GetService(typeof(FakeService));

            Assert.IsType(typeof(FakeService), service1.GetType());
            Assert.IsType(typeof(FakeService), service2.GetType());
            Assert.NotSame(service1, service2);
        }

        [TestMethod]
        public void ServicesRegisteredWithImplementationTypeForSingletons()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service1 = serviceProvider.GetService(typeof(IFakeService));
            var service2 = serviceProvider.GetService(typeof(IFakeService));

            Assert.IsType(typeof(FakeService), service1.GetType());
            Assert.IsType(typeof(FakeService), service2.GetType());
            Assert.Same(service1, service2);
        }

        [TestMethod]
        public void ServicesRegisteredWithNoInterfaceTypeForSingletons()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(FakeService))
                .BuildServiceProvider();

            var service1 = serviceProvider.GetService(typeof(FakeService));
            var service2 = serviceProvider.GetService(typeof(FakeService));

            Assert.IsType(typeof(FakeService), service1.GetType());
            Assert.IsType(typeof(FakeService), service2.GetType());
            Assert.Same(service1, service2);
        }

        [TestMethod]
        public void ServiceInstanceCanBeResolved()
        {
            var instance = new FakeService();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), instance)
                .BuildServiceProvider();

            var service = serviceProvider.GetService(typeof(IFakeService));

            Assert.IsType(typeof(FakeService), service.GetType());
            Assert.Same(instance, service);
        }

        [TestMethod]
        public void TransientServiceCanBeResolvedFromProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service1 = serviceProvider.GetService(typeof(IFakeService));
            var service2 = serviceProvider.GetService(typeof(IFakeService));

            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotSame(service1, service2);
        }

        [TestMethod]
        public void ServiceInstanceCanBeArrayResolved()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            object[] service = serviceProvider.GetService(new Type[] { typeof(IFakeService) });

            Assert.NotNull(service);
            Assert.Equals(1, service.Length);
            Assert.IsType(typeof(FakeService), service[0].GetType());
        }

        [TestMethod]
        public void NonexistentServiceCanBeArrayResolved()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            object[] service = serviceProvider.GetService(new Type[] { typeof(INonexistentService) });

            Assert.Empty(service);
        }

        [TestMethod]
        public void SelfResolveThenDispose()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service = serviceProvider.GetService(typeof(IServiceProvider));

            Assert.NotNull(service);

            serviceProvider.Dispose();
        }

        [TestMethod]
        public void AttemptingToResolveNonexistentServiceReturnsNull()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service = serviceProvider.GetService(typeof(INonexistentService));

            Assert.Null(service);
        }
    }
}