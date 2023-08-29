//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Specifies the lifetime of a service in an <see cref="IServiceCollection"/>.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// Specifies that a single instance of the service will be created.
        /// </summary>
        Singleton,

        /// <summary>
        /// Specifies that a new instance of the service will be created every time it is requested.
        /// </summary>
        Transient,

        /// <summary>
        /// 
        /// </summary>
        Scoped
    }
}
