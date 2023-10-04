//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Diagnostics;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// An <see cref="IServiceScope" /> implementation that implements <see cref="IDisposable" />.
    /// </summary>
    [DebuggerDisplay("{ServiceProvider,nq}")]
    public readonly struct ServiceScope : IServiceScope
    {
        private readonly IServiceScope _serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceScope"/> struct.
        /// Wraps an instance of <see cref="IServiceScope" />.
        /// </summary>
        /// <param name="serviceScope">The <see cref="IServiceScope"/> instance to wrap.</param>
        public ServiceScope(IServiceScope serviceScope)
        {
            if (serviceScope == null)
            {
                throw new ArgumentNullException();
            }

            _serviceScope = serviceScope;
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

        /// <inheritdoc />
        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}