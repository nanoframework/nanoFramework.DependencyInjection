using System;
using System.Collections;

using nanoFramework.TestFramework;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceProviderExtensionsTest
    {
        [TestMethod]
        public void GetService_Returns_CorrectService()
        {
            var serviceProvider = CreateTestServiceProvider(1);
            var service = serviceProvider.GetService(typeof(IFoo));

            Assert.IsType(typeof(Foo1), service);
        }

        [TestMethod]
        public void GetRequiredService_Returns_CorrectService()
        {
            var serviceProvider = CreateTestServiceProvider(1);
            var service = serviceProvider.GetRequiredService(typeof(IFoo));

            Assert.IsType(typeof(Foo1), service);
        }

        [TestMethod]
        public void GetRequiredService_Throws_WhenNoServiceRegistered()
        {
            var serviceProvider = CreateTestServiceProvider(0);

            Assert.Throws(typeof(InvalidOperationException), () => serviceProvider.GetRequiredService(typeof(IFoo)),
                $"No service for type '{typeof(IFoo)}' has been registered.");
        }

        [TestMethod]
        public void GetServices_Returns_MultipleServices()
        {
            var serviceProvider = CreateTestServiceProvider(4);

            var types = new Type[2] { typeof(IFoo), typeof(IBar) };
            object[] services = ((ServiceProvider)serviceProvider).GetService(types);

            Assert.IsType(typeof(Foo1), services[0].GetType());
            Assert.IsType(typeof(Foo2), services[1].GetType());
            Assert.IsType(typeof(Bar1), services[2].GetType());
            Assert.IsType(typeof(Bar2), services[3].GetType());
            Assert.Equal(4, services.Length);
        }

        [TestMethod]
        public void GetServices_Returns_AllServices()
        {
            var serviceProvider = CreateTestServiceProvider(2);
            object[] services = serviceProvider.GetServices(typeof(IFoo));

            Assert.IsType(typeof(Foo1), services[0].GetType());
            Assert.IsType(typeof(Foo2), services[1].GetType());
            Assert.Equal(2, services.Length);
        }

        [TestMethod]
        public void GetServices_Returns_SingleService()
        {
            var serviceProvider = CreateTestServiceProvider(1);
            object[] services = serviceProvider.GetServices(typeof(IFoo));

            Assert.IsType(typeof(Foo1), services[0].GetType());
            Assert.Equal(1, services.Length);
        }

        [TestMethod]
        public void GetServices_Returns_CorrectTypes()
        {
            var serviceProvider = CreateTestServiceProvider(4);
            object[] services = serviceProvider.GetServices(typeof(IBar));

            Assert.IsType(typeof(Bar1), services[0].GetType());
            Assert.IsType(typeof(Bar2), services[1].GetType());
            Assert.Equal(2, services.Length);
        }

        [TestMethod]
        public void GetServices_Returns_EmptyArray_WhenNoServicesAvailable()
        {
            var serviceProvider = CreateTestServiceProvider(0);
            object[] services = serviceProvider.GetServices(typeof(IFoo));

            //Assert.IsType(typeof(object[]), services);
            Assert.Equal(0, services.Length);
        }

        [TestMethod]
        public void GetServices_WithBuildServiceProvider_Returns_EmptyList_WhenNoServicesAvailable()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IFoo), new ArrayList());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            object[] services = serviceProvider.GetServices(typeof(IBar));

            //Assert.IsType(typeof(object[]), services.GetType());
            Assert.Equal(0, services.Length);
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