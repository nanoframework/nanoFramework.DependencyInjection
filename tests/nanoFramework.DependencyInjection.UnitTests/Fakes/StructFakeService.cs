using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public struct StructFakeService : IStructFakeService
    {
        public StructFakeService(IServiceProvider serviceProvider)
        {
        }
    }
}
