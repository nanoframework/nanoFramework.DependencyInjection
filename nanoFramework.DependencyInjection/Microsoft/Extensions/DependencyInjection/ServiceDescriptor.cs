//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service Factory method delegate
    /// </summary>
    public delegate object ImplementationFactoryDelegate(IServiceProvider serviceProvider);

    /// <summary>
    /// Describes a service with its service type, implementation, and lifetime.
    /// </summary>
    [DebuggerDisplay("Lifetime = {Lifetime}, ServiceType = {ServiceType}, ImplementationType = {ImplementationType}")]
    public class ServiceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="implementationType"/>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="implementationType">The <see cref="Type"/> implementing the service.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="implementationType"/> can't be null</exception>
        /// <exception cref="ArgumentException">Implementation type cannot be an abstract or interface class.</exception>
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
            : this(serviceType, lifetime)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException();
            }

            if (implementationType.IsAbstract || implementationType.IsInterface)
            {
                throw new ArgumentException();
            }

            ImplementationType = implementationType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="instance"/>
        /// as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="instance">The instance implementing the service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="instance"/> can't be <see langword="null"/></exception>
        public ServiceDescriptor(Type serviceType, object instance)
            : this(serviceType, ServiceLifetime.Singleton)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            ImplementationInstance = instance;
            ImplementationType = GetImplementationType();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="factory"/>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="factory">A factory used for creating service instances.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="implementationType"/> can't be null</exception>
        /// <exception cref="ArgumentException">Implementation type cannot be an abstract or interface class.</exception>
        public ServiceDescriptor(Type serviceType, ImplementationFactoryDelegate factory, ServiceLifetime lifetime)
            : this(serviceType, lifetime)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            ImplementationFactory = factory ?? throw new ArgumentNullException();
        }

        private ServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        /// <summary>
        /// The <see cref="ServiceLifetime"/> of the service.
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// The <see cref="Type"/> of the service.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// The <see cref="Type"/> implementing the service.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// The instance of the implementation.
        /// </summary>
        public object ImplementationInstance { get; set; }

        /// <summary>
        /// Gets the factory used for creating service instances.
        /// </summary>
        public ImplementationFactoryDelegate ImplementationFactory { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            string lifetime = $"{nameof(ServiceType)}: {ServiceType} {nameof(Lifetime)}: {Lifetime} ";

            if (ImplementationType != null)
            {
                return lifetime + $"{nameof(ImplementationType)}: {ImplementationType}";
            }

            return lifetime + $"{nameof(ImplementationInstance)}: {ImplementationInstance}";
        }

        /// <summary>
        /// Returns the <see cref="Type"/> implementing the instance.
        /// </summary>
        public Type GetImplementationType()
        {
            if (ImplementationType != null)
            {
                return ImplementationType;
            }
            else if (ImplementationInstance != null)
            {
                return ImplementationInstance.GetType();
            }

            Debug.Assert(false, "ImplementationType and ImplementationInstance must be non null");

            return null;
        }

        /// <summary>
        /// Creates an instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationType"/>,
        /// and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns>A new instance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor Describe(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime);
        }
    }
}
