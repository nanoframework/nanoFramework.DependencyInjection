//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="ServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionServiceExtensions
    {
        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with an
        /// implementation of the type specified in <paramref name="implementationType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton);
            services.Add(descriptor);

            return services;
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            return services.AddSingleton(serviceType, serviceType);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with an
        /// instance specified in <paramref name="implementationInstance"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationInstance">The instance of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, object implementationInstance)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            var serviceDescriptor = new ServiceDescriptor(serviceType, implementationInstance);
            services.Add(serviceDescriptor);

            return services;
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> with an
        /// implementation of the type specified in <paramref name="implementationType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            var descriptor = new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient);
            services.Add(descriptor);

            return services;
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            return services.AddTransient(serviceType, serviceType);
        }

        /// <summary>
        /// Adds the specified <paramref name="descriptor"/> to the <paramref name="collection"/> if the
        /// service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="descriptor"/> can't be <see langword="null"/>.</exception>
        public static void TryAdd(this IServiceCollection collection, ServiceDescriptor descriptor)
        {
            if (collection == null)
            {
                throw new ArgumentNullException();
            }

            if (descriptor == null)
            {
                throw new ArgumentNullException();
            }

            int count = collection.Count;

            for (int index = 0; index < count; index++)
            {
                if (collection[index].ServiceType == descriptor.ServiceType)
                {
                    // Already added
                    return;
                }
            }

            collection.Add(descriptor);
        }

        /// <summary>
        /// Adds a <see cref="ServiceDescriptor"/> if an existing descriptor with the same
        /// <see cref="ServiceDescriptor.ServiceType"/> and an implementation that does not already exist
        /// in <paramref name="services."/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/>.</param>
        /// <remarks>
        /// Use <see cref="TryAddEnumerable(IServiceCollection, ServiceDescriptor)"/> when registering a service implementation of a
        /// service type that
        /// supports multiple registrations of the same service type. Using
        /// <see cref="IServiceCollection.Add(ServiceDescriptor)"/> is not idempotent and can add
        /// duplicate
        /// <see cref="ServiceDescriptor"/> instances if called twice. Using
        /// <see cref="TryAddEnumerable(IServiceCollection, ServiceDescriptor)"/> will prevent registration
        /// of multiple implementation types.
        /// </remarks>
        /// <exception cref="ArgumentException">Implementation type cannot be 'implementationType' because it is indistinguishable from other services registered for 'descriptor.ServiceType'.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="descriptor"/> can't be <see langword="null"/>.</exception>
        public static void TryAddEnumerable(this IServiceCollection services, ServiceDescriptor descriptor)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (descriptor == null)
            {
                throw new ArgumentNullException();
            }

            Type implementationType = descriptor.GetImplementationType();

            if (implementationType == typeof(object)
                || implementationType == descriptor.ServiceType)
            {
                throw new ArgumentException();
            }

            int count = services.Count;

            for (int index = 0; index < count; index++)
            {
                ServiceDescriptor service = services[index];

                if (service.ServiceType == descriptor.ServiceType
                    && service.GetImplementationType() == implementationType)
                {
                    // Already added
                    return;
                }
            }

            services.Add(descriptor);
        }

        /// <summary>
        /// Adds the specified <see cref="ServiceDescriptor"/>s if an existing descriptor with the same
        /// <see cref="ServiceDescriptor.ServiceType"/> and an implementation that does not already exist
        /// in <paramref name="services."/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptors">The <see cref="ServiceDescriptor"/>s.</param>
        /// <remarks>
        /// Use <see cref="TryAddEnumerable(IServiceCollection, ServiceDescriptor)"/> when registering a service
        /// implementation of a service type that
        /// supports multiple registrations of the same service type. Using
        /// <see cref="IServiceCollection.Add(ServiceDescriptor)"/> is not idempotent and can add
        /// duplicate
        /// <see cref="ServiceDescriptor"/> instances if called twice. Using
        /// <see cref="TryAddEnumerable(IServiceCollection, ServiceDescriptor)"/> will prevent registration
        /// of multiple implementation types.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="descriptors"/> can't be <see langword="null"/>.</exception>
        public static void TryAddEnumerable(this IServiceCollection services, IEnumerable descriptors)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (descriptors == null)
            {
                throw new ArgumentNullException();
            }

            foreach (ServiceDescriptor descriptor in descriptors)
            {
                services.TryAddEnumerable(descriptor);
            }
        }

        /// <summary>
        /// Removes the first service in <see cref="IServiceCollection"/> with the same service type
        /// as <paramref name="descriptor"/> and adds <paramref name="descriptor"/> to the collection.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> to replace with.</param>
        /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="descriptor"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection Replace(this IServiceCollection collection, ServiceDescriptor descriptor)
        {
            if (collection == null)
            {
                throw new ArgumentNullException();
            }

            if (descriptor == null)
            {
                throw new ArgumentNullException();
            }

            // Remove existing
            int count = collection.Count;

            for (int index = 0; index < count; index++)
            {
                if (collection[index].ServiceType == descriptor.ServiceType)
                {
                    collection.RemoveAt(index);
                    break;
                }
            }

            collection.Add(descriptor);

            return collection;
        }

        /// <summary>
        /// Removes all services of type <paramref name="serviceType"/> in <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="serviceType">The service type to remove.</param>
        /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> can't be <see langword="null"/>.</exception>
        public static IServiceCollection RemoveAll(this IServiceCollection collection, Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            for (int index = collection.Count - 1; index >= 0; index--)
            {
                ServiceDescriptor descriptor = collection[index];

                if (descriptor.ServiceType == serviceType)
                {
                    collection.RemoveAt(index);
                }
            }

            return collection;
        }
    }
}
