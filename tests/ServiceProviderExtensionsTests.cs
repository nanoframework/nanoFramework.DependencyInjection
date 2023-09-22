//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Collections;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceProviderExtensionsTests
    {
        [TestMethod]
        public void GetServiceReturnsCorrectService()
        {
            var serviceProvider = CreateTestServiceProvider(1);
            var service = serviceProvider.GetService(typeof(IFoo));

            Assert.IsType(typeof(Foo1), service);
        }

        [TestMethod]
        public void GetRequiredServiceReturnsCorrectService()
        {
            var serviceProvider = CreateTestServiceProvider(1);
            var service = serviceProvider.GetRequiredService(typeof(IFoo));

            Assert.IsType(typeof(Foo1), service);
        }

        [TestMethod]
        public void GetRequiredServiceThrowsWhenNoServiceRegistered()
        {
            var serviceProvider = CreateTestServiceProvider(0);

            var expectedMessage = $"No service for type 'nanoFramework.DependencyInjection.UnitTests.ServiceProviderExtensionsTest+IFoo' has been registered.";
            Assert.ThrowsException(typeof(InvalidOperationException),
                () => serviceProvider.GetRequiredService(typeof(IFoo))
            );
        }

        [TestMethod]
        public void GetServicesReturnsMultipleServices()
        {
            var serviceProvider = CreateTestServiceProvider(4);

            var types = new Type[] { typeof(IFoo), typeof(IBar) };
            object[] services = ((ServiceProvider)serviceProvider).GetService(types);

            Assert.IsType(typeof(Foo1), services[0].GetType());
            Assert.IsType(typeof(Foo2), services[1].GetType());
            Assert.IsType(typeof(Bar1), services[2].GetType());
            Assert.IsType(typeof(Bar2), services[3].GetType());
            Assert.AreEqual(4, services.Length);
        }

        [TestMethod]
        public void GetServicesReturnsAllServices()
        {
            var serviceProvider = CreateTestServiceProvider(2);
            object[] services = serviceProvider.GetServices(typeof(IFoo));

            Assert.IsType(typeof(Foo1), services[0].GetType());
            Assert.IsType(typeof(Foo2), services[1].GetType());
            Assert.AreEqual(2, services.Length);
        }

        [TestMethod]
        public void GetServicesReturnsSingleService()
        {
            var serviceProvider = CreateTestServiceProvider(1);
            object[] services = serviceProvider.GetServices(typeof(IFoo));

            Assert.IsType(typeof(Foo1), services[0].GetType());
            Assert.AreEqual(1, services.Length);
        }

        [TestMethod]
        public void GetServicesReturnsCorrectTypes()
        {
            var serviceProvider = CreateTestServiceProvider(4);
            object[] services = serviceProvider.GetServices(typeof(IBar));

            Assert.IsType(typeof(Bar1), services[0].GetType());
            Assert.IsType(typeof(Bar2), services[1].GetType());
            Assert.AreEqual(2, services.Length);
        }

        [TestMethod]
        public void GetServicesReturnsEmptyArrayWhenNoServicesAvailable()
        {
            var serviceProvider = CreateTestServiceProvider(0);
            object[] services = serviceProvider.GetServices(typeof(IFoo));

            //Assert.IsType(typeof(object[]), services);
            Assert.AreEqual(0, services.Length);
        }

        [TestMethod]
        public void GetServicesWithBuildServiceProviderReturnsEmptyListWhenNoServicesAvailable()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IFoo), new ArrayList());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            object[] services = serviceProvider.GetServices(typeof(IBar));

            //Assert.IsType(typeof(object[]), services.GetType());
            Assert.AreEqual(0, services.Length);
        }


        private static IServiceProvider CreateTestServiceProvider(int count)
        {
            var serviceCollection = new ServiceCollection();

            if (count > 0)
            {
                serviceCollection.AddTransient(typeof(IFoo), typeof(Foo1));
            }

            if (count > 1)
            {
                serviceCollection.AddTransient(typeof(IFoo), typeof(Foo2));
            }

            if (count > 2)
            {
                serviceCollection.AddTransient(typeof(IBar), typeof(Bar1));
            }

            if (count > 3)
            {
                serviceCollection.AddTransient(typeof(IBar), typeof(Bar2));
            }

            return serviceCollection.BuildServiceProvider();
        }

        public interface IFoo { }

        public class Foo1 : IFoo { }

        public class Foo2 : IFoo { }

        public interface IBar { }

        public class Bar1 : IBar { }

        public class Bar2 : IBar { }
    }
}
