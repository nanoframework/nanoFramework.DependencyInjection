// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;

using nanoFramework.TestFramework;
using nanoFramework.DependencyInjection.UnitTests.Fakes;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceCollectionTests
    {
        [TestMethod]
        public static void ServiceDescriptorDescribe()
        {
            var serviceDescriptor = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            Assert.IsType(typeof(IFakeObject), serviceDescriptor.ServiceType.GetType());
            Assert.IsType(typeof(FakeObject), serviceDescriptor.ImplementationType.GetType());
            Assert.True(serviceDescriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceDescriptorToString()
        {
            var expectedMessage = "ServiceType: nanoFramework.DependencyInjection.UnitTests.Fakes.IFakeObject Lifetime: 0 ImplementationType: nanoFramework.DependencyInjection.UnitTests.Fakes.FakeObject.";
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            Assert.IsType(typeof(IFakeObject), serviceDescriptor.ServiceType.GetType());
            Assert.IsType(typeof(FakeObject), serviceDescriptor.ImplementationType.GetType());
            Assert.True(serviceDescriptor.Lifetime == ServiceLifetime.Singleton);
            Assert.Contains(serviceDescriptor.ToString(), expectedMessage);
        }

        [TestMethod]
        public static void ServiceCollectionAdd()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor);
            Assert.Equal(1, serviceCollection.Count);

            Assert.IsType(typeof(IFakeObject), serviceCollection[0].ServiceType.GetType());
            Assert.IsType(typeof(FakeObject), serviceCollection[0].ImplementationType.GetType());
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionReplace()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor);
            Assert.Equal(1, serviceCollection.Count);

            Assert.IsType(typeof(IFakeObject), serviceCollection[0].ServiceType.GetType());
            Assert.IsType(typeof(FakeObject), serviceCollection[0].ImplementationType.GetType());
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);

            serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.Replace(serviceDescriptor);
            Assert.Equal(1, serviceCollection.Count);

            Assert.IsType(typeof(IFakeObject), serviceCollection[0].ServiceType.GetType());
            Assert.IsType(typeof(FakeObject), serviceCollection[0].ImplementationType.GetType());
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
        }

        [TestMethod]
        public static void ServiceCollectionAddSingleton()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(IFakeObject), typeof(FakeObject));
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionAddTransient()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(IFakeObject), typeof(FakeObject));
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
        }

        [TestMethod]
        public static void ServiceCollectionTryAdd()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.TryAdd(serviceDescriptor);
            serviceCollection.TryAdd(serviceDescriptor);
            Assert.Equal(1, serviceCollection.Count);

            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
        }

        [TestMethod]
        public static void ServiceCollectionTryAddEnumerable()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.TryAddEnumerable(serviceDescriptor);
            Assert.Equal(1, serviceCollection.Count);

            var serviceDescriptors = new ArrayList
            {
                new ServiceDescriptor(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Singleton)
            };

            serviceCollection.TryAddEnumerable(serviceDescriptors);
            Assert.Equal(2, serviceCollection.Count);
        }

        [TestMethod]
        public static void ServiceCollectionInterfaceThrowsArgumentException()
        {
            var serviceCollection = new ServiceCollection();

            Assert.Throws(typeof(ArgumentException),
                    () => serviceCollection.AddSingleton(typeof(IFakeObject), typeof(IFakeObject))
                );
        }

        [TestMethod]
        public static void ServiceCollectionRemove()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);

            serviceCollection.Add(serviceDescriptor);
            Assert.Equal(1, serviceCollection.Count);
            
            serviceCollection.Remove(serviceDescriptor);
            Assert.True(serviceCollection.Count == 0);
        }

        [TestMethod]
        public static void ServiceCollectionRemoveAll()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeService), typeof(FakeService));
            Assert.Equal(3, serviceCollection.Count);

            serviceCollection.RemoveAll(typeof(IFakeObject));
            Assert.Equal(1, serviceCollection.Count);

            Assert.IsType(typeof(IFakeService), serviceCollection[0].ServiceType.GetType());
            Assert.IsType(typeof(FakeService), serviceCollection[0].ImplementationType.GetType());
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }

        [TestMethod]
        public static void ServiceCollectionRemoveAt()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeObject), typeof(FakeObject));
            serviceCollection.AddSingleton(typeof(IFakeService), typeof(FakeService));
            Assert.Equal(3, serviceCollection.Count);

            serviceCollection.RemoveAt(1);
            Assert.Equal(2, serviceCollection.Count);

            Assert.IsType(typeof(IFakeObject), serviceCollection[0].ServiceType.GetType());
            Assert.IsType(typeof(FakeObject), serviceCollection[0].ImplementationType.GetType());
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Transient);
            Assert.IsType(typeof(IFakeService), serviceCollection[1].ServiceType.GetType());
            Assert.IsType(typeof(FakeService), serviceCollection[1].ImplementationType.GetType());
            Assert.True(serviceCollection[1].Lifetime == ServiceLifetime.Singleton);
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
            Assert.Equal(3, serviceCollection.Count);

            Assert.True(serviceCollection[0].ServiceType.GetType() == typeof(IFakeService));
            Assert.True(serviceCollection[0].ImplementationType.GetType() == typeof(FakeService));
            Assert.True(serviceCollection[0].Lifetime == ServiceLifetime.Singleton);
        }


        [TestMethod]
        public static void ServiceCollectionContains()
        {
            var serviceCollection = new ServiceCollection();
            var serviceDescriptor1 = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Transient);
            var serviceDescriptor2 = ServiceDescriptor.Describe(typeof(IFakeObject), typeof(FakeObject), ServiceLifetime.Singleton);

            serviceCollection.Add(serviceDescriptor1);
            Assert.Equal(1, serviceCollection.Count);

            Assert.True(serviceCollection.Contains(serviceDescriptor1));
            Assert.False(serviceCollection.Contains(serviceDescriptor2));
        }
    }
}