//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace System
{
    /// <summary>
    /// Defines scope for <see cref="IServiceProvider"/>.
    /// </summary>
    public interface IServiceProviderScope : IServiceProvider, IDisposable
    {
    }
}
