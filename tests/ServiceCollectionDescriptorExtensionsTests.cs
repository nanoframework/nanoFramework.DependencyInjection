using System;
using Microsoft.Extensions.DependencyInjection;
using nanoFramework.DependencyInjection.UnitTests.Fakes;
using nanoFramework.TestFramework;

// ReSharper disable InvokeAsExtensionMethod
namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceCollectionDescriptorExtensionsTests
    {
        private static ServiceDescriptor CreateSingletonServiceDescriptor() => ServiceDescriptor.Singleton(typeof(IService1), typeof(Service1));
        private static ServiceDescriptor CreateTransientServiceDescriptor() => ServiceDescriptor.Transient(typeof(IService1), typeof(Service1));

        [TestMethod]
        public void Add_adds_descriptor()
        {
            var descriptor = CreateSingletonServiceDescriptor();

            var collection = new ServiceCollection();

            ServiceCollectionDescriptorExtensions.Add(collection, descriptor);

            Assert.AreEqual(1, collection.Count);
            Assert.IsTrue(collection.Contains(descriptor));
        }

        [TestMethod]
        public void Add_adds_descriptors()
        {
            var singletonDescriptor = CreateSingletonServiceDescriptor();
            var transientDescriptor = CreateTransientServiceDescriptor();
            var descriptors = new[] { singletonDescriptor, transientDescriptor };

            var collection = new ServiceCollection();

            ServiceCollectionDescriptorExtensions.Add(collection, descriptors);

            Assert.AreEqual(2, collection.Count);
            Assert.IsTrue(collection.Contains(singletonDescriptor));
            Assert.IsTrue(collection.Contains(transientDescriptor));
        }

        [TestMethod]
        public void Add_throws_when_collection_is_null()
        {
            var test = new object[]{};
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.Add(null!, CreateSingletonServiceDescriptor()));
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.Add(null!, new ServiceDescriptor[]{}));
        }

        [TestMethod]
        public void Add_throws_when_descriptor_is_null()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.Add(new ServiceCollection(), ((ServiceDescriptor)null)!));
        }

        [TestMethod]
        public void Add_throws_when_descriptors_contains_invalid_type()
        {
            var descriptors = new[]
            {
                CreateSingletonServiceDescriptor(),
                CreateTransientServiceDescriptor(),
                new object()
            };

            Assert.ThrowsException(typeof(ArgumentException), () => ServiceCollectionDescriptorExtensions.Add(new ServiceCollection(), descriptors));
        }

        [TestMethod]
        public void Add_throws_when_descriptors_is_null()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.Add(new ServiceCollection(), ((ServiceDescriptor[])null)!));
        }

        [TestMethod]
        public void RemoveAll_removes_descriptors()
        {
            var serviceA = ServiceDescriptor.Scoped(typeof(IService1), typeof(Service1));
            var serviceB = ServiceDescriptor.Singleton(typeof(IService1), typeof(Service1));
            var serviceC = ServiceDescriptor.Transient(typeof(IService1), typeof(Service1));
            var serviceD = ServiceDescriptor.Singleton(typeof(IService2), typeof(Service2));

            var collection = new ServiceCollection();

            collection.Add(serviceA);
            collection.Add(serviceB);
            collection.Add(serviceC);
            collection.Add(serviceD);

            Assert.AreEqual(4, collection.Count);
            Assert.IsTrue(collection.Contains(serviceA));
            Assert.IsTrue(collection.Contains(serviceB));
            Assert.IsTrue(collection.Contains(serviceC));
            Assert.IsTrue(collection.Contains(serviceD));

            ServiceCollectionDescriptorExtensions.RemoveAll(collection, typeof(IService1));

            Assert.AreEqual(1, collection.Count);
            Assert.IsFalse(collection.Contains(serviceA));
            Assert.IsFalse(collection.Contains(serviceB));
            Assert.IsFalse(collection.Contains(serviceC));
            Assert.IsTrue(collection.Contains(serviceD));
        }

        [TestMethod]
        public void RemoveAll_throws_when_collection_is_null()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.RemoveAll(null!, typeof(IService1)));
        }

        [TestMethod]
        public void RemoveAll_throws_when_serviceType_is_null()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.RemoveAll(new ServiceCollection(), null!));
        }

        [TestMethod]
        public void Replace_replaces_descriptor()
        {
            var singletonDescriptor = CreateSingletonServiceDescriptor();
            var transientDescriptor = CreateTransientServiceDescriptor();
            var otherDescriptor = ServiceDescriptor.Singleton(typeof(IService2), typeof(Service2));

            var collection = new ServiceCollection();

            collection.Add(singletonDescriptor);
            collection.Add(otherDescriptor);

            Assert.AreEqual(2, collection.Count);
            Assert.IsTrue(collection.Contains(singletonDescriptor));
            Assert.IsFalse(collection.Contains(transientDescriptor));
            Assert.IsTrue(collection.Contains(otherDescriptor));

            ServiceCollectionDescriptorExtensions.Replace(collection, transientDescriptor);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.Contains(singletonDescriptor));
            Assert.IsTrue(collection.Contains(transientDescriptor));
            Assert.IsTrue(collection.Contains(otherDescriptor));
        }

        [TestMethod]
        public void Replace_throws_when_collection_is_null()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.Replace(null!, CreateSingletonServiceDescriptor()));
        }

        [TestMethod]
        public void Replace_throws_when_descriptor_is_null()
        {
            Assert.ThrowsException(typeof(ArgumentNullException), () => ServiceCollectionDescriptorExtensions.Replace(new ServiceCollection(), null!));
        }
    }
}
