//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using Microsoft.Extensions.DependencyInjection;
using nanoFramework.DependencyInjection.UnitTests.Fakes;
using nanoFramework.TestFramework;
using System;
using System.Collections;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceCollectionTests
    {
        [TestMethod]
        public static void ServiceDescriptorDescribe()
        {
            var serviceDescriptor = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            Assert.IsInstanceOfType(serviceDescriptor.ServiceType, typeof(IFakeObject));
            Assert.IsInstanceOfType(serviceDescriptor.ImplementationType, typeof(FakeObject));
            Assert.IsTrue(serviceDescriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceDescriptorToString()
        {
            var expectedMessage = "ServiceType: nanoFramework.DependencyInjection.UnitTests.Fakes.IFakeObject Lifetime: 0 ImplementationType: nanoFramework.DependencyInjection.UnitTests.Fakes.FakeObject.";
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            Assert.IsInstanceOfType(serviceDescriptor.ServiceType, typeof(IFakeObject));
            Assert.IsInstanceOfType(serviceDescriptor.ImplementationType, typeof(FakeObject));
            Assert.IsTrue(serviceDescriptor.Lifetime == ServiceLifetime.Singleton);
            Assert.Contains(serviceDescriptor.ToString(), expectedMessage);
        }

        [TestMethod]
        public static void ServiceCollectionAdd()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor);
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsInstanceOfType(serviceCollection[0].ServiceType, typeof(IFakeObject));
            Assert.IsInstanceOfType(serviceCollection[0].ImplementationType, typeof(FakeObject));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionReplace()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor);
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsInstanceOfType(serviceCollection[0].ServiceType, typeof(IFakeObject));
            Assert.IsInstanceOfType(serviceCollection[0].ImplementationType, typeof(FakeObject));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);

            serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.Replace(serviceDescriptor);
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsInstanceOfType(serviceCollection[0].ServiceType, typeof(IFakeObject));
            Assert.IsInstanceOfType(serviceCollection[0].ImplementationType, typeof(FakeObject));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
        }

        [TestMethod]
        public static void ServiceCollectionAddSingleton()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(IFakeObject), typeof(FakeObject));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionAddTransient()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(IFakeObject), typeof(FakeObject));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
        }

        [TestMethod]
        public static void ServiceCollectionTryAdd()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.TryAdd(serviceDescriptor);
            serviceCollection.TryAdd(serviceDescriptor);
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
        }

        [TestMethod]
        public static void ServiceCollectionTryAddEnumerable()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.TryAddEnumerable(serviceDescriptor);
            Assert.AreEqual(1, serviceCollection.Count);

            var serviceDescriptors = new ArrayList
            {
                new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Singleton)
            };

            serviceCollection.TryAddEnumerable(serviceDescriptors);
            Assert.AreEqual(2, serviceCollection.Count);
        }

        [TestMethod]
        public static void ServiceCollectionInterfaceThrowsArgumentException()
        {
            var serviceCollection = new ServiceCollection();

            Assert.ThrowsException(typeof(ArgumentException),
                    () => serviceCollection.AddSingleton(typeof(IFakeObject), typeof(IFakeObject))
                );
        }

        [TestMethod]
        public static void ServiceCollectionRemove()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.Add(serviceDescriptor);
            Assert.AreEqual(1, serviceCollection.Count);

            serviceCollection.Remove(serviceDescriptor);
            Assert.IsTrue(serviceCollection.Count == 0);
        }

        [TestMethod]
        public static void ServiceCollectionRemoveAll()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeService), typeof(FakeService));
            Assert.AreEqual(3, serviceCollection.Count);

            serviceCollection.RemoveAll(typeof(IFakeObject));
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsInstanceOfType(serviceCollection[0].ServiceType, typeof(IFakeService));
            Assert.IsInstanceOfType(serviceCollection[0].ImplementationType, typeof(FakeService));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionRemoveAt()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeService), typeof(FakeService));
            Assert.AreEqual(3, serviceCollection.Count);

            serviceCollection.RemoveAt(1);
            Assert.AreEqual(2, serviceCollection.Count);

            Assert.IsInstanceOfType(serviceCollection[0].ServiceType, typeof(IFakeObject));
            Assert.IsInstanceOfType(serviceCollection[0].ImplementationType, typeof(FakeObject));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
            Assert.IsInstanceOfType(serviceCollection[1].ServiceType, typeof(IFakeService));
            Assert.IsInstanceOfType(serviceCollection[1].ImplementationType, typeof(FakeService));
            Assert.IsTrue(serviceCollection[1].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionInsert()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor1 = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);
            var serviceDescriptor2 = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);
            var serviceDescriptor3 = ServiceDescriptor.Describe(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor1);
            serviceCollection.Add(serviceDescriptor2);
            serviceCollection.Insert(0, serviceDescriptor3);
            Assert.AreEqual(3, serviceCollection.Count);

            Assert.IsInstanceOfType(serviceCollection[0].ServiceType, typeof(IFakeService));
            Assert.IsInstanceOfType(serviceCollection[0].ImplementationType, typeof(FakeService));
            Assert.IsTrue(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionContains()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor1 = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);
            var serviceDescriptor2 = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor1);
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsTrue(serviceCollection.Contains(serviceDescriptor1));
            Assert.IsFalse(serviceCollection.Contains(serviceDescriptor2));
        }
    }
}
