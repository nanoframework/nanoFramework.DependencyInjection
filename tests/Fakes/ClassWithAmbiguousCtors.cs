//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class ClassWithAmbiguousCtors
    {
        public ClassWithAmbiguousCtors(string data)
        {
            CtorUsed = "string";
        }

        public ClassWithAmbiguousCtors(IFakeService service, string data)
        {
            CtorUsed = "IFakeService, string";
        }

        public ClassWithAmbiguousCtors(IFakeService service, int data)
        {
            CtorUsed = "IFakeService, int";
        }

        public ClassWithAmbiguousCtors(IFakeService service, string data1, int data2)
        {
            FakeService = service;
            Data1 = data1;
            Data2 = data2;

            CtorUsed = "IFakeService, string, string";
        }

        public IFakeService FakeService { get; }

        public string Data1 { get; }

        public int Data2 { get; }
        public string CtorUsed { get; set; }
    }
}
