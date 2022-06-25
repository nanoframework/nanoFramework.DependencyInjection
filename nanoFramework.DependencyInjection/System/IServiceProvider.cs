// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>. -or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        object GetService(Type serviceType);

        /// <summary>
        /// Gets the service objects of the specified type.
        /// </summary>
        /// <param name="serviceType">An array object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object array of type <paramref name="serviceType"/>. -or- array empty if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        object[] GetService(Type[] serviceType);
    }
}