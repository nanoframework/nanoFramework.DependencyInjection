//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Defines scope for <see cref="IServiceProvider"/>.
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> used to resolve dependencies from the scope.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}