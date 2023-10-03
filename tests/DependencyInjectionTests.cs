//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.DependencyInjection.UnitTests.Fakes;
using nanoFramework.TestFramework;
using System;

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
            Assert.AreNotSame(service1, service2);
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
        public void ServiceInstanceWithPrimitiveBinding()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(ClassWithPrimitiveBinding))
                .BuildServiceProvider();

            var service = (ClassWithPrimitiveBinding)serviceProvider.GetService(typeof(ClassWithPrimitiveBinding));

            Assert.IsNull(service.Str);
            Assert.IsNull(service.Obj);
            Assert.IsNotNull(service.Guid);

            Assert.IsTrue(service.Boolean == false);   
            Assert.IsTrue(service.Short == 0);
            Assert.IsTrue(service.Ushort == 0);
            Assert.IsTrue(service.Int == 0);
            Assert.IsTrue(service.UInt == 0);
            Assert.IsTrue(service.Long == 0);
            Assert.IsTrue(service.Ulong == 0);
            Assert.IsTrue(service.Double == 0);
            Assert.IsTrue(service.Float == 0);
            Assert.IsTrue(service.Byte == new byte());
            Assert.IsTrue(service.SByte == new sbyte());
            Assert.IsTrue(service.DateTime == new DateTime());
            Assert.IsTrue(service.TimeSpan == new TimeSpan());
            Assert.IsTrue(service.Array == default);
            Assert.IsTrue(service.ArrayList == default);

            //TODO: Add array types - reflection is not resolving with the IsArray flag
            // https://github.com/nanoframework/Home/issues/1086
        }

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
            Assert.AreNotSame(service1, service2);
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
            Assert.AreSame(service1, service2);
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
            Assert.AreSame(service1, service2);
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
            Assert.AreEqual(0, rootService.IntProperty);
            Assert.AreEqual(null, rootService.StringProperty);

            var innerService = (Service3)serviceProvider.GetRequiredService(typeof(IService3));
            Assert.IsNotNull(innerService);
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
            Assert.AreSame(instance, service);
        }

        [TestMethod]
        public void TransientServiceCanBeResolvedFromProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service1 = serviceProvider.GetService(typeof(IFakeService));
            var service2 = serviceProvider.GetService(typeof(IFakeService));

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void ServiceInstanceCanBeArrayResolved()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            object[] service = serviceProvider.GetService(new Type[] { typeof(IFakeService) });

            Assert.IsNotNull(service);
            Assert.AreEqual(1, service.Length);
            Assert.IsType(typeof(FakeService), service[0].GetType());
        }

        [TestMethod]
        public void NonexistentServiceCanBeArrayResolved()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            object[] service = serviceProvider.GetService(new Type[] { typeof(INonexistentService) });

            CollectionAssert.Empty(service);
        }

        [TestMethod]
        public void DoesNotAllowForAmbiguousConstructorMatches()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .AddSingleton(typeof(ClassWithAmbiguousCtors))
                .BuildServiceProvider();

            Assert.ThrowsException(typeof(InvalidOperationException),
                () => serviceProvider.GetService(typeof(ClassWithAmbiguousCtors))
            );
        }

        [TestMethod]
        public void IngoreUnresolvedMultipleConstructorMatches()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .AddSingleton(typeof(ClassWithMultipleCtors))
                .BuildServiceProvider();

            var service = (ClassWithMultipleCtors)serviceProvider.GetService(typeof(ClassWithMultipleCtors));

            Assert.AreEqual(2, service.CtorUsed);
        }

        [TestMethod]
        public void SelfResolveThenDispose()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service = (IDisposable)serviceProvider.GetService(typeof(IServiceProvider));

            Assert.IsNotNull(service);

            serviceProvider.Dispose();
        }

        [TestMethod]
        public void SelfResolveAndDisposeThrowsObjectDisposedException()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            serviceProvider.Dispose();

            Assert.ThrowsException(typeof(ObjectDisposedException),
                () => serviceProvider.GetService(typeof(IServiceProvider)));
        }


        [TestMethod]
        public void SafelyDisposeNestedProviderReferences()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(ClassWithNestedReferencesToProvider))
                .BuildServiceProvider();

            var service = (IDisposable)serviceProvider.GetService(typeof(ClassWithNestedReferencesToProvider));

            Assert.IsNotNull(service);
            service.Dispose();
        }


        [TestMethod]
        public void AttemptingToResolveNonexistentServiceReturnsNull()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

            var service = serviceProvider.GetService(typeof(INonexistentService));

            Assert.IsNull(service);
        }


        [TestMethod]
        public void ServiceRegisteredWithScopedReturnsSameInstanceWithinScope()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();


            using var scope = serviceProvider.CreateScope();
            var service1 = scope.ServiceProvider.GetService(typeof(IFakeService));
            var service2 = scope.ServiceProvider.GetService(typeof(IFakeService));

            Assert.AreSame(service1, service2);

        }

        [TestMethod]
        public void ServiceRegisteredWithScopedReturnsDifferentInstanceOutsideScope()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();


            using var scope1 = serviceProvider.CreateScope();
            using var scope2 = serviceProvider.CreateScope();
            var service1 = scope1.ServiceProvider.GetService(typeof(IFakeService));
            var service2 = scope2.ServiceProvider.GetService(typeof(IFakeService));

            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void ServiceRegisteredWithScopedIsDisposedWhenScopeIsDisposed()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();


            FakeService service1, service2;
            using (var scope1 = serviceProvider.CreateScope())
            {
                using (var scope2 = serviceProvider.CreateScope())
                {
                    service1 = (FakeService)scope1.ServiceProvider.GetService(typeof(IFakeService));
                    service2 = (FakeService)scope2.ServiceProvider.GetService(typeof(IFakeService));
                }

                Assert.IsTrue(service2.Disposed);
                Assert.IsFalse(service1.Disposed);
            }
        }

        [TestMethod]
        public void ServiceRegisteredWithScopedReturnsNullWhenNoScope()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();


            var service = serviceProvider.GetService(typeof(IFakeService));

            Assert.IsNull(service);
        }

        [TestMethod]
        public void ServiceRegisteredWithScopedThrowsExceptionWhenValidateScopesEnabledAndNoScope()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider(new ServiceProviderOptions{ValidateScopes = true});


            Assert.ThrowsException(typeof(InvalidOperationException),
                () => serviceProvider.GetService(typeof(IFakeService)));
        }

        [TestMethod]
        public void NoDuplicateServiceRegisteredWithScoped()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();


            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider.GetServices(typeof(IFakeService));

            Assert.AreEqual(1, services.Length);
        }
    }
}
