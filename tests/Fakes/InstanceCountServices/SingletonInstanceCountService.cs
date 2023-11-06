namespace nanoFramework.DependencyInjection.UnitTests.Fakes.InstanceCountServices
{
    internal interface ISingletonInstanceCountService
    {
        int Id { get; }
    }

    internal class SingletonInstanceCountService : ISingletonInstanceCountService
    {
        private readonly ITransientInstanceCountService _transientInstanceCountService;
        private readonly object _syncLock = new();

        public SingletonInstanceCountService(ITransientInstanceCountService transientInstanceCountService)
        {
            _transientInstanceCountService = transientInstanceCountService;

            lock (_syncLock)
            {
                InstanceCount += 1;
                Id = InstanceCount;
            }
        }

        public int Id { get; }
        public static int InstanceCount { get; set; }
    }
}
