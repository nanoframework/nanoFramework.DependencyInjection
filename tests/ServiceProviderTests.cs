using nanoFramework.TestFramework;
using nanoFramework.DependencyInjection.UnitTests.Fakes.InstanceCountServices;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ServiceProviderTests
    {
        [Cleanup]
        public void Cleanup()
        {
            SingletonInstanceCountService.InstanceCount = 0;
            TransientInstanceCountService.InstanceCount = 0;
        }

        [TestMethod]
        public void GetService_should_only_create_a_single_instance_of_transient_dependencies()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(ISingletonInstanceCountService), typeof(SingletonInstanceCountService));
            serviceCollection.AddTransient(typeof(ITransientInstanceCountService), typeof(TransientInstanceCountService));

            var sut = serviceCollection.BuildServiceProvider();

            // Act
            var service = (ISingletonInstanceCountService) sut.GetRequiredService(typeof(ISingletonInstanceCountService));

            // Assert
            Assert.AreEqual(1, SingletonInstanceCountService.InstanceCount);
            Assert.AreEqual(1, TransientInstanceCountService.InstanceCount);
        }
    }
}
