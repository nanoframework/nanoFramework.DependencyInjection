//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class ClassWithThrowingCtor
    {
        public ClassWithThrowingCtor(FakeService service)
        {
            throw new Exception(nameof(ClassWithThrowingCtor));
        }
    }
}
