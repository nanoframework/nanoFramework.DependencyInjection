//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class AnotherClass
    {
        public AnotherClass(IFakeService fakeService)
        {
            FakeService = fakeService;
        }

        public IFakeService FakeService { get; }
    }
}
