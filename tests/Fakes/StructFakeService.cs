//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public struct StructFakeService : IStructFakeService
    {
        private readonly IFakeObject _fakeObject;

        public StructFakeService(IFakeObject fakeobject)
        {
            _fakeObject = fakeobject;
        }
    }
}
