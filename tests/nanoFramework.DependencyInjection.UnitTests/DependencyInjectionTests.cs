// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using nanoFramework.TestFramework;
using nanoFramework.DependencyInjection.UnitTests.Fakes;
using System.Diagnostics;

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

        // TODO:  typeof(StructFakeService).GetConstructor() is failing to create struct correctly.
        // https://github.com/nanoframework/Home/issues/1085

        //[TestMethod]
        //public void ServicesRegisteredWithImplementationTypeForStructSingletonServices()
        //{
        //    var serviceProvider = new ServiceCollection()
        //        .AddSingleton(typeof(IFakeObject), typeof(FakeObject))
        //        .AddSingleton(typeof(IStructFakeService), typeof(StructFakeService))
        //        .BuildServiceProvider();

        //    var service = serviceProvider.GetService(typeof(IStructFakeService));

        //    Assert.IsType(typeof(StructFakeService), service.GetType());
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
        public void NestedServicesRegisteredWithInterfaceTypeForSingletons()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IService1), typeof(Service1))
                .AddSingleton(typeof(IService2), typeof(Service2))
                .AddSingleton(typeof(IService3), typeof(Service3))
                .AddSingleton(typeof(IRootService), typeof(RootService))
                .BuildServiceProvider();

            var rootService = (RootService)serviceProvider.GetRequiredService(typeof(IRootService));

            Assert.IsType(typeof(Service1), rootService.Service1.GetType());
            Assert.IsType(typeof(Service2), rootService.Service2.GetType());
            Assert.Equal(2000, rootService.IntProperty);
            Assert.Equal("default", rootService.StringProperty);

            var innerService = (Service3)serviceProvider.GetRequiredService(typeof(IService3));
            Assert.NotNull(innerService);
        }

        [TestMethod]
        public void LastServiceReplacesPreviousServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeMultipleService), typeof(FakeOneMultipleService))
                .AddTransient(typeof(IFakeMultipleService), typeof(FakeTwoMultipleService))
                .BuildServiceProvider();

            var service = (FakeTwoMultipleService)serviceProvider.GetService(typeof(IFakeMultipleService));

            Assert.IsType(typeof(FakeTwoMultipleService), service.GetType());
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
            Assert.Equal(1, service.Length);
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
        public void DoesNotAllowForAmbiguousConstructorMatches()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .AddSingleton(typeof(ClassWithAmbiguousCtors))
                .BuildServiceProvider();

            var expectedMessage = "Multiple constructors accepting all given argument types have been found in type 'nanoFramework.DependencyInjection.UnitTests.Fakes.ClassWithAmbiguousCtors'. There should only be one applicable constructor.";
            Assert.Throws(typeof(InvalidOperationException),
                () =>
                {
                    try
                    {
                        serviceProvider.GetService(typeof(ClassWithAmbiguousCtors));
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (string.Equals(expectedMessage, ex.Message))
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            );
        }

        [TestMethod]
        public void SelfResolveThenDispose()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service = (IDisposable)serviceProvider.GetService(typeof(IServiceProvider));

            Assert.NotNull(service);

            serviceProvider.Dispose();
        }

        [TestMethod]
        public void SafelyDisposeNestedProviderReferences()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(ClassWithNestedReferencesToProvider))
                .BuildServiceProvider();

            var service = (IDisposable)serviceProvider.GetService(typeof(ClassWithNestedReferencesToProvider));

            Assert.NotNull(service);
            //service.Dispose();  //TODO: Not working throwing memory error 
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