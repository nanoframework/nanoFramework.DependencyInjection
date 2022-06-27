//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    internal class RootService : IRootService
    {
        private readonly IService1 _service1;
        private readonly IService2 _service2;

        public string StringProperty { get; set; }

        public int IntProperty { get; set; }

        internal IService1 Service1 => _service1;

        internal IService2 Service2 => _service2;

        public RootService(IService1 service1, IService2 service2)
            : this(service1, service2, "default", 2000)
        {
        }

        public RootService(IService1 service1, IService2 service2, string stringProperty, int intProperty)
        {
            if (service1 == null)
            {
                throw new ArgumentNullException(nameof(service1));
            }

            if (service2 == null)
            {
                throw new ArgumentNullException(nameof(service2));
            }

            _service1 = service1;
            _service2 = service2;
            StringProperty = stringProperty;
            IntProperty = intProperty;
        }
    }
}
