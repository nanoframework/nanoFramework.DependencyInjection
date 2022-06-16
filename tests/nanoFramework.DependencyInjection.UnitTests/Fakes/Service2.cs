using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    internal class Service2 : IService2
    {
        public IService3 Service3 { get; private set; }

        public Service2(IService3 service3)
        {
            if (service3 == null)
            {
                throw new ArgumentNullException("service3");
            }

            Service3 = service3;
        }
    }
}
