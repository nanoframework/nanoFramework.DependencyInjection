//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System.Collections;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class ClassWithMultipleCtors
    {
        public ClassWithMultipleCtors(string data)
        {
            CtorUsed = 0;
        }

        public ClassWithMultipleCtors(IFakeService service, string data)
        {
            CtorUsed = 1;
        }

        public ClassWithMultipleCtors(IFakeService service, string data1, int data2)
        {
            FakeService = service;
            Data1 = data1;
            Data2 = data2;

            CtorUsed = 2;
        }

        public ClassWithMultipleCtors(IFakeService service, ICollection collection, string data1, int data2)
        {
            FakeService = service;
            Data1 = data1;
            Data2 = data2;
            Collection = collection;
            CtorUsed = 3;
        }

        public IFakeService FakeService { get; }

        public string Data1 { get; }

        public int Data2 { get; }
        
        public int CtorUsed { get; set; }

        public ICollection Collection { get; set; }
    }
}
