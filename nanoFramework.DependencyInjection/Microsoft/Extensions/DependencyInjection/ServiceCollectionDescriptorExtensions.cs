using System;
using System.Collections;

#nullable enable
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding and removing services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionDescriptorExtensions
    {
        /// <summary>
        /// Adds the specified <paramref name="descriptor"/> to the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> to add.</param>
        /// <returns>A reference to the current instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection Add(this IServiceCollection collection, ServiceDescriptor descriptor)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(descriptor);

            collection.Add(descriptor);
            return collection;
        }

        /// <summary>
        /// Adds a sequence of <see cref="ServiceDescriptor"/> to the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptors">The <see cref="ServiceDescriptor"/>s to add.</param>
        /// <returns>A reference to the current instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection Add(this IServiceCollection collection, IEnumerable descriptors)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(descriptors);

            foreach (var d in descriptors)
            {
                if (d is not ServiceDescriptor descriptor)
                {
                    throw new ArgumentException();
                }

                collection.Add(descriptor);
            }

            return collection;
        }

        /// <summary>
        /// Adds the specified <paramref name="descriptor"/> to the <paramref name="collection"/> if the
        /// service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> to add.</param>
        public static void TryAdd(this IServiceCollection collection, ServiceDescriptor descriptor)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(descriptor);

            var count = collection.Count;
            for (var i = 0; i < count; i++)
            {
                if (collection[i].ServiceType == descriptor.ServiceType)
                {
                    // Already added
                    return;
                }
            }

            collection.Add(descriptor);
        }

        /// <summary>
        /// Adds the specified <paramref name="descriptors"/> to the <paramref name="collection"/> if the
        /// service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptors">The <see cref="ServiceDescriptor"/>s to add.</param>
        public static void TryAdd(this IServiceCollection collection, IEnumerable descriptors)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(descriptors);

            foreach (var d in descriptors)
            {
                if (d is not ServiceDescriptor descriptor)
                {
                    throw new ArgumentException();
                }

                collection.TryAdd(descriptor);
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Transient"/> service
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        public static void TryAddTransient(this IServiceCollection collection, Type service)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);

            var descriptor = ServiceDescriptor.Transient(service, service);
            TryAdd(collection, descriptor);
        }

        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Transient"/> service
        /// with the <paramref name="implementationType"/> implementation
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        public static void TryAddTransient(this IServiceCollection collection, Type service, Type implementationType)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);
            ThrowHelper.ThrowIfNull(implementationType);

            var descriptor = ServiceDescriptor.Transient(service, implementationType);
            TryAdd(collection, descriptor);
        }

        // TODO: Implement this when ImplementationType is properly handled for factory registrations
        /*
        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Transient"/> service
        /// using the factory specified in <paramref name="implementationFactory"/>
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        public static void TryAddTransient(this IServiceCollection collection, Type service, Func<IServiceProvider, object> implementationFactory)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);
            ThrowHelper.ThrowIfNull(implementationFactory);

            var descriptor = ServiceDescriptor.Transient(service, implementationFactory);
            TryAdd(collection, descriptor);
        }
        */


        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Scoped"/> service
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        public static void TryAddScoped(this IServiceCollection collection, Type service)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);

            var descriptor = ServiceDescriptor.Scoped(service, service);
            TryAdd(collection, descriptor);
        }

        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Scoped"/> service
        /// with the <paramref name="implementationType"/> implementation
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        public static void TryAddScoped(this IServiceCollection collection, Type service, Type implementationType)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);
            ThrowHelper.ThrowIfNull(implementationType);

            var descriptor = ServiceDescriptor.Scoped(service, implementationType);
            TryAdd(collection, descriptor);
        }

        // TODO: Implement this when ImplementationType is properly handled for factory registrations
        /*
        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Scoped"/> service
        /// using the factory specified in <paramref name="implementationFactory"/>
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        public static void TryAddScoped(this IServiceCollection collection, Type service, Func<IServiceProvider, object> implementationFactory)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);
            ThrowHelper.ThrowIfNull(implementationFactory);

            var descriptor = ServiceDescriptor.Scoped(service, implementationFactory);
            TryAdd(collection, descriptor);
        }
        */

        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Singleton"/> service
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        public static void TryAddSingleton(this IServiceCollection collection, Type service)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);

            var descriptor = ServiceDescriptor.Singleton(service, service);
            TryAdd(collection, descriptor);
        }

        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Singleton"/> service
        /// with the <paramref name="implementationType"/> implementation
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        public static void TryAddSingleton(this IServiceCollection collection, Type service, Type implementationType)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);
            ThrowHelper.ThrowIfNull(implementationType);

            var descriptor = ServiceDescriptor.Singleton(service, implementationType);
            TryAdd(collection, descriptor);
        }

        // TODO: Implement this when ImplementationType is properly handled for factory registrations
        /*
        /// <summary>
        /// Adds the specified <paramref name="service"/> as a <see cref="ServiceLifetime.Singleton"/> service
        /// using the factory specified in <paramref name="implementationFactory"/>
        /// to the <paramref name="collection"/> if the service type hasn't already been registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="service">The type of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        public static void TryAddSingleton(this IServiceCollection collection, Type service, Func<IServiceProvider, object> implementationFactory)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(service);
            ThrowHelper.ThrowIfNull(implementationFactory);

            var descriptor = ServiceDescriptor.Singleton(service, implementationFactory);
            TryAdd(collection, descriptor);
        }
        */

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
        /// <see cref="Add(IServiceCollection, ServiceDescriptor)"/> is not idempotent and can add
        /// duplicate
        /// <see cref="ServiceDescriptor"/> instances if called twice. Using
        /// <see cref="TryAddEnumerable(IServiceCollection, ServiceDescriptor)"/> will prevent registration
        /// of multiple implementation types.
        /// </remarks>
        public static void TryAddEnumerable(this IServiceCollection services, ServiceDescriptor descriptor)
        {
            ThrowHelper.ThrowIfNull(services);
            ThrowHelper.ThrowIfNull(descriptor);

            var implementationType = descriptor.GetImplementationType();

            if (implementationType == typeof(object) || implementationType == descriptor.ServiceType)
            {
                throw new ArgumentException();
            }

            var count = services.Count;
            for (var i = 0; i < count; i++)
            {
                var service = services[i];
                if (service.ServiceType == descriptor.ServiceType && service.GetImplementationType() == implementationType)
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
        /// Use <see cref="TryAddEnumerable(IServiceCollection, IEnumerable)"/> when registering a service
        /// implementation of a service type that
        /// supports multiple registrations of the same service type. Using
        /// <see cref="Add(IServiceCollection, IEnumerable)"/> is not idempotent and can add
        /// duplicate
        /// <see cref="ServiceDescriptor"/> instances if called twice. Using
        /// <see cref="TryAddEnumerable(IServiceCollection, IEnumerable)"/> will prevent registration
        /// of multiple implementation types.
        /// </remarks>
        public static void TryAddEnumerable(this IServiceCollection services, IEnumerable descriptors)
        {
            ThrowHelper.ThrowIfNull(services);
            ThrowHelper.ThrowIfNull(descriptors);

            foreach (var d in descriptors)
            {
                if (d is not ServiceDescriptor descriptor)
                {
                    throw new ArgumentException();
                }

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
        public static IServiceCollection Replace(this IServiceCollection collection, ServiceDescriptor descriptor)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(descriptor);

            // Remove existing
            var count = collection.Count;
            for (var i = 0; i < count; i++)
            {
                if (collection[i].ServiceType == descriptor.ServiceType)
                {
                    collection.RemoveAt(i);
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
        public static IServiceCollection RemoveAll(this IServiceCollection collection, Type serviceType)
        {
            ThrowHelper.ThrowIfNull(collection);
            ThrowHelper.ThrowIfNull(serviceType);

            for (var i = collection.Count - 1; i >= 0; i--)
            {
                var descriptor = collection[i];
                if (descriptor.ServiceType == serviceType)
                {
                    collection.RemoveAt(i);
                }
            }

            return collection;
        }
    }

}
