//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class CreationCountFakeService
    {
        public static readonly object InstanceLock = new object();

        public CreationCountFakeService(IFakeService dependency)
        {
            InstanceCount++;
            InstanceId = InstanceCount;
        }

        public static int InstanceCount { get; set; }

        public int InstanceId { get; }
    }
}
