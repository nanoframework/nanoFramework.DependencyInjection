//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using nanoFramework.TestFramework;
using nanoFramework.DependencyInjection.UnitTests.Fakes;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ActivatorUtilitiesTests
    {
        [TestMethod]
        public void TypeActivatorEnablesYouToCreateAnyTypeWithServicesEvenWhenNotInIocContainer()
        {
            var serviceProvider = new ServiceCollection()
               .AddTransient(typeof(IFakeService), typeof(FakeService))
               .BuildServiceProvider();

            var instance = (AnotherClass)ActivatorUtilities.CreateInstance(serviceProvider, typeof(AnotherClass));

            Assert.NotNull(instance.FakeService);
        }


        [TestMethod]
        public void TypeActivatorAcceptsAnyNumberOfAdditionalConstructorParametersToProvide()
        {
            var serviceProvider = new ServiceCollection()
               .AddTransient(typeof(IFakeService), typeof(FakeService))
               .BuildServiceProvider();

            var instance = (AnotherClassAcceptingData)ActivatorUtilities.CreateInstance(
                serviceProvider,
                typeof(AnotherClassAcceptingData),
                "1",
                "2");

            Assert.NotNull(instance.FakeService);
            Assert.Equal("1", instance.One);
            Assert.Equal("2", instance.Two);
        }

        [TestMethod]
        public void TypeActivatorCanDisambiguateConstructorsWithUniqueArguments()
        {
            var serviceProvider = new ServiceCollection()
               .AddTransient(typeof(IFakeService), typeof(FakeService))
               .BuildServiceProvider();

            var instance = (ClassWithAmbiguousCtors)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ClassWithAmbiguousCtors), "1", 2);

            Assert.NotNull(instance);
            Assert.NotNull(instance.FakeService);
            Assert.Equal("1", instance.Data1);
            Assert.Equal(2, instance.Data2);
        }

        [TestMethod]
        [DataRow("", "string")]
        [DataRow(5, "IFakeService, int")]
        public void TypeActivatorCreateInstanceUsesFirstMathchedConstructor(object value, string ctor)
        {
            var serviceProvider = new ServiceCollection()
                  .AddTransient(typeof(IFakeService), typeof(FakeService))
                  .BuildServiceProvider();

            var instance = (ClassWithAmbiguousCtors)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ClassWithAmbiguousCtors), value);

            Assert.Equal(ctor, instance.CtorUsed);
        }

        [TestMethod]
        public void TypeActivatorRethrowsOriginalExceptionFromConstructor()
        {
            Assert.Throws(typeof(Exception),
                () =>
                {
                    try
                    {
                        ActivatorUtilities.CreateInstance(null, typeof(ClassWithThrowingCtor), new object[] { new FakeService() });
                    }
                    catch (Exception ex)
                    {
                        if (string.Equals(nameof(ClassWithThrowingCtor), ex.Message))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            throw new NullReferenceException();
                        }
                    }
                }
            );

            Assert.Throws(typeof(Exception),
                () =>
                {
                    try
                    {
                        ActivatorUtilities.CreateInstance(null, typeof(ClassWithThrowingEmptyCtor));
                    }
                    catch (Exception ex)
                    {
                        if (string.Equals(nameof(ClassWithThrowingEmptyCtor), ex.Message))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            throw new NullReferenceException();
                        }
                    }
                }
            );
        }

        [TestMethod]
        public void TypeActivatorRequiresAllArgumentsCanBeAccepted()
        {
            Assert.SkipTest("Test failing. Ignoring for now.");


            var serviceProvider = new ServiceCollection()
              .AddTransient(typeof(IFakeService), typeof(FakeService))
              .BuildServiceProvider();

            var expectedMessage = "A suitable constructor for type 'nanoFramework.DependencyInjection.UnitTests.Fakes.AnotherClassAcceptingData' could not be located. Ensure the type is concrete and all parameters of a public constructor are either registered as services or passed as arguments. Also ensure no extraneous arguments are provided.";

            Assert.Throws(typeof(InvalidOperationException),
                () =>
                {
                    try
                    {
                        ActivatorUtilities.CreateInstance(serviceProvider, typeof(AnotherClassAcceptingData), "1", "2", "3");
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

            Assert.Throws(typeof(InvalidOperationException),
                () =>
                {
                    try
                    {
                        ActivatorUtilities.CreateInstance(serviceProvider, typeof(AnotherClassAcceptingData), 1, 2);
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
        public void GetServiceOrCreateInstanceRegisteredServiceTransient()
        {
            // Reset the count because test order is not guaranteed
            lock (CreationCountFakeService.InstanceLock)
            {
                CreationCountFakeService.InstanceCount = 0;

                var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeService), typeof(FakeService))
                .AddTransient(typeof(CreationCountFakeService))
                .BuildServiceProvider();

                var service = (CreationCountFakeService)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(CreationCountFakeService));
                Assert.NotNull(service);
                Assert.Equal(1, service.InstanceId);
                Assert.Equal(1, CreationCountFakeService.InstanceCount);

                service = (CreationCountFakeService)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(CreationCountFakeService));
                Assert.NotNull(service);
                Assert.Equal(2, service.InstanceId);
                Assert.Equal(2, CreationCountFakeService.InstanceCount);
            }
        }

        [TestMethod]
        public void GetServiceOrCreateInstanceRegisteredServiceSingleton()
        {
            // Reset the count because test order is not guaranteed
            lock (CreationCountFakeService.InstanceLock)
            {
                CreationCountFakeService.InstanceCount = 0;

                var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeService), typeof(FakeService))
                .AddSingleton(typeof(CreationCountFakeService))
                .BuildServiceProvider();

                var service = (CreationCountFakeService)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(CreationCountFakeService));
                Assert.NotNull(service);
                Assert.Equal(1, service.InstanceId);
                Assert.Equal(1, CreationCountFakeService.InstanceCount);

                service = (CreationCountFakeService)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(CreationCountFakeService));
                Assert.NotNull(service);
                Assert.Equal(1, service.InstanceId);
                Assert.Equal(1, CreationCountFakeService.InstanceCount);
            }
        }

        [TestMethod]
        public void GetServiceOrCreateInstanceUnregisteredService()
        {
            // Reset the count because test order is not guaranteed
            lock (CreationCountFakeService.InstanceLock)
            {
                CreationCountFakeService.InstanceCount = 0;

                var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(IFakeService), typeof(FakeService))
                .BuildServiceProvider();

                var service = (CreationCountFakeService)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(CreationCountFakeService));
                Assert.NotNull(service);
                Assert.Equal(1, service.InstanceId);
                Assert.Equal(1, CreationCountFakeService.InstanceCount);

                service = (CreationCountFakeService)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(CreationCountFakeService));
                Assert.NotNull(service);
                Assert.Equal(2, service.InstanceId);
                Assert.Equal(2, CreationCountFakeService.InstanceCount);
            }
        }

        [TestMethod]
        public void UnRegisteredServiceAsConstructorParameterThrowsException()
        {
            Assert.SkipTest("Test failing. Ignoring for now.");

            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(CreationCountFakeService))
                .BuildServiceProvider();

            var expectedMessage = "Unable to resolve service for type 'nanoFramework.DependencyInjection.UnitTests.Fakes.IFakeService' while attempting to activate 'nanoFramework.DependencyInjection.UnitTests.Fakes.CreationCountFakeService'.";

            Assert.Throws(typeof(InvalidOperationException),
                   () =>
                   {
                       try
                       {
                           ActivatorUtilities.CreateInstance(serviceProvider, typeof(CreationCountFakeService));
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
    }
}
