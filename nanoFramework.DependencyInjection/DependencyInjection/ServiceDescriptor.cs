// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace nanoFramework.DependencyInjection
{
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
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
            : this(serviceType, lifetime)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (implementationType.IsAbstract || implementationType.IsInterface)
            {
                throw new ArgumentException("Implementation type cannot be an abstract or interface class.");
            }

            ImplementationType = implementationType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="instance"/>
        /// as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="instance">The instance implementing the service.</param>
        public ServiceDescriptor(Type serviceType, object instance)
            : this(serviceType, ServiceLifetime.Singleton)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            ImplementationInstance = instance;
            ImplementationType = GetImplementationType();
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

        internal Type GetImplementationType()
        {
            if (ImplementationType != null)
            {
                return ImplementationType;
            }
            else if (ImplementationInstance != null)
            {
                return ImplementationInstance.GetType();
            }

            Debug.Assert(false, "ImplementationType, ImplementationInstance or ImplementationFactory must be non null");

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
