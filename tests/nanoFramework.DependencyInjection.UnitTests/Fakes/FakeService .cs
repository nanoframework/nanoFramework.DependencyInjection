using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class FakeService : IFakeService,  IDisposable
    {
        public PocoClass Value { get; set; }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(FakeService));
            }

            Disposed = true;
        }
    }
}