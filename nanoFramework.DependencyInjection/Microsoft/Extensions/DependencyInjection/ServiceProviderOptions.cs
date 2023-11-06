//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Options for configuring various behaviors of the default <see cref="IServiceProvider"/> implementation.
    /// </summary>
    public class ServiceProviderOptions
    {
        // Avoid allocating objects in the default case
        internal static readonly ServiceProviderOptions Default = new ServiceProviderOptions();

        /// <summary>
        /// <see langword="true"/> to perform check verifying that scoped services never gets resolved from root provider; otherwise <see langword="false"/>. Defaults to <see langword="false"/>.
        /// </summary>
        public bool ValidateScopes { get; set; }

        /// <summary>
        /// <see langword="true"/> to perform check verifying that all services can be created during BuildServiceProvider call; otherwise <see langword="false"/>. Defaults to <see langword="false"/>.
        /// </summary>
        public bool ValidateOnBuild { get; set; }
    }
}
