//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Extension methods for getting services from an <see cref="IServiceProvider" />.
    /// </summary>
    public static class ServiceProviderServiceExtensions
    {
        /// <summary>
        /// Get an enumeration of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An array of services of type <paramref name="serviceType"/>.</returns>
        public static object[] GetServices(this IServiceProvider provider, Type serviceType)
        {
            if (provider == null)
            {
                throw new ArgumentNullException();
            }

            return provider.GetService(new Type[] { serviceType });
        }

        /// <summary>
        /// Get an enumeration of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
        /// <param name="serviceType">An array of <paramref name="serviceType"/> object that specifies the type of service object to get.</param>
        /// <returns>An array of services of type <paramref name="serviceType"/>.</returns>
        /// <exception cref="ArgumentNullException">'provider' or 'serviceType' can't be <see langword="null"/>.</exception>
        public static object[] GetServices(this IServiceProvider provider, Type[] serviceType)
        {
            if (provider == null)
            {
                throw new ArgumentNullException();
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            return provider.GetService(serviceType);
        }

        /// <summary>
        /// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType"/>.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type <paramref name="serviceType"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> or <paramref name="serviceType"></paramref> can't be <see langword="null"/>.</exception>
        public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
        {
            if (provider == null)
            {
                throw new ArgumentNullException();
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            object service = provider.GetService(serviceType);

            if (service == null)
            {
                throw new InvalidOperationException();
            }

            return service;
        }

        /// <summary>
        /// Creates a new <see cref="ServiceScope"/> that can be used to resolve scoped services.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> to create the scope from.</param>
        /// <returns>An <see cref="ServiceScope"/> that can be used to resolve scoped services.</returns>
        public static IServiceScope CreateScope(this IServiceProvider provider)
        {
            return new ServiceScope(provider.CreateScope());
        }
    }
}