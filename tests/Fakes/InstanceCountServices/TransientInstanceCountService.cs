namespace nanoFramework.DependencyInjection.UnitTests.Fakes.InstanceCountServices
{
    internal interface ITransientInstanceCountService
    {
        int Id { get; }
    }

    internal class TransientInstanceCountService : ITransientInstanceCountService
    {
        private readonly object _syncLock = new();

        public TransientInstanceCountService()
        {
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
