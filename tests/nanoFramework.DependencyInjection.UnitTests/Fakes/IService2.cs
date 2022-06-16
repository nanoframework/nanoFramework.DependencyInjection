using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    internal interface IService2
    {
        IService3 Service3 { get; }
    }
}
