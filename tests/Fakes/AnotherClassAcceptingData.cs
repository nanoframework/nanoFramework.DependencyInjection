//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class AnotherClassAcceptingData
    {
        public AnotherClassAcceptingData(IFakeService fakeService, string one, string two)
        {
            FakeService = fakeService;
            One = one;
            Two = two;
        }

        public IFakeService FakeService { get; }

        public string One { get; }

        public string Two { get; }
    }
}
