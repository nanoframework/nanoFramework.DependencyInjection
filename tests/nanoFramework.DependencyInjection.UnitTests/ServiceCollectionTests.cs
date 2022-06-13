using System;
using nanoFramework.DependencyInjection.UnitTests.Fakes;
using nanoFramework.TestFramework;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceCollectionTests
    {
        [TestMethod]
        public static void CreateServiceDescriptor()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(TestObject), typeof(TestObject), ServiceLifetime.Transient);

            serviceCollection.Add(serviceDescriptor);
            Assert.True(serviceCollection.Count == 1);

            Assert.True(serviceCollection[0].ServiceType.GetType() == typeof(TestObject));
            Assert.True(serviceCollection[0].ImplementationType.GetType() == typeof(TestObject));
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Transient);

            serviceCollection.Remove(serviceDescriptor);
            Assert.True(serviceCollection.Count == 0);
        }
    }

    public class TestObject
    {

    }
}