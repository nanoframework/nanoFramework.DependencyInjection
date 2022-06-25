// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

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