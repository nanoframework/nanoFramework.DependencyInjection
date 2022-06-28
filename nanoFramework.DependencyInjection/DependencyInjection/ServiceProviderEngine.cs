//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Reflection;
using System.Collections;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Defines an engine for managing <see cref="IServiceCollection"/> services that provides custom support to other objects.
    /// </summary>
    internal sealed class ServiceProviderEngine
    {
        internal static ServiceProviderEngine Instance { get; } = new ServiceProviderEngine();

        private ServiceProviderEngine() { }

        /// <summary>
        /// A collection of implemented services.
        /// </summary>
        internal IServiceCollection Services { get; set; }

        /// <summary>
        /// Validate service by attempting to activate all dependent services.
        /// </summary>
        internal void ValidateService(ServiceDescriptor descriptor)
        {
            GetService(descriptor.GetImplementationType());
        }

        /// <summary>
        /// Dispose of all <see cref="IDisposable"/> service descriptors in services. 
        /// </summary>
        internal void DisposeServices()
        {
            foreach (ServiceDescriptor descriptor in Services)
            {
                if (descriptor.ServiceType == typeof(IServiceProvider))
                {
                    continue;
                }

                if (descriptor.ImplementationInstance is IDisposable instance)
                {
                    instance.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the last added service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        internal object GetService(Type serviceType)
        {
            var services = GetServiceObjects(serviceType);

            if (services.Length == 0)
            {
                return null;
            }

            // returns the last added service of this type
            return services[services.Length - 1];
        }

        /// <summary>
        /// Gets the service objects of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> can't be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="serviceType"/> can't be empty.</exception>
        internal object[] GetService(Type[] serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException();
            }

            if (serviceType.Length == 0)
            {
                throw new ArgumentException();
            }

            // optimized for single item service type
            if (serviceType.Length == 1)
            {
                var services = GetServiceObjects(serviceType[0]);

                if (services.Length > 0)
                {
                    return services;
                }

                return new object[0];
            }

            // multiple service type items 
            object[] array = new object[0];

            foreach (Type type in serviceType)
            {
                var services = GetServiceObjects(type);

                if (services.Length > 0)
                {
                    var newArray = new object[array.Length + services.Length];

                    Array.Copy(array, newArray, array.Length);
                    Array.Copy(services, 0, newArray, array.Length, services.Length);

                    array = newArray;
                }
            }

            return array.Length != 0 ? array : new object[0];
        }

        /// <summary>
        /// Gets the service objects of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        private object[] GetServiceObjects(Type serviceType)
        {
            ArrayList services = new ArrayList();

            foreach (ServiceDescriptor descriptor in Services)
            {
                if (descriptor.ServiceType == serviceType)
                {
                    if (descriptor.Lifetime == ServiceLifetime.Singleton
                        && descriptor.ImplementationInstance != null)
                    {
                        services.Add(descriptor.ImplementationInstance);
                    }
                    else
                    {
                        var instance = Resolve(descriptor.ImplementationType);
                        {
                            descriptor.ImplementationInstance = instance;
                        }

                        services.Add(instance);
                    }
                }
            }

            return (object[])services.ToArray(typeof(object));
        }

        /// <summary>
        /// Resolve and activates the specified implementation type.
        /// </summary>
        /// <param name="implementationType">An object that specifies the implementation type of service object to get.</param>
        /// <exception cref="InvalidOperationException">A suitable constructor for type <paramref name="implementationType"/> could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.</exception>
        /// <exception cref="InvalidOperationException">Unable to resolve service for type <paramref name="implementationType"/> while attempting to activate.</exception>
        private object Resolve(Type implementationType)
        {
            object instance;

            ParameterInfo[] constructorParameters = GetParameters(implementationType);

            if (constructorParameters == null)
            {
                throw new InvalidOperationException();
            }

            if (constructorParameters.Length == 0)
            {
                instance = Activator.CreateInstance(implementationType);
            }
            else
            {
                Type[] types = new Type[constructorParameters.Length];
                object[] parameters = new object[constructorParameters.Length];

                for (int index = 0; index < constructorParameters.Length; index++)
                {
                    var parameterType = constructorParameters[index].ParameterType;

                    if (TryBindToPrimitive(parameterType, out object defaultType))
                    {
                        types[index] = parameterType;
                        parameters[index] = defaultType;
                    }
                    else
                    {
                        var service = GetService(parameterType);

                        if (service == null)
                        {
                            throw new InvalidOperationException();
                        }

                        types[index] = parameterType;
                        parameters[index] = service;
                    }
                }

                instance = Activator.CreateInstance(implementationType, types, parameters);
            }

            return instance;
        }

        /// <summary>
        /// Gets the parameters from the constructor with the most parameters.
        /// </summary>
        /// <param name="implementationType">An object that specifies the implementation type of service object to get.</param>
        /// <exception cref="InvalidOperationException">Multiple constructors accepting all given argument types have been found in type <paramref name="implementationType"/>. There should only be one applicable constructor.</exception>
        private ParameterInfo[] GetParameters(Type implementationType)
        {
            int bestLength = -1;

            ParameterInfo[] bestParameters = null;
            ConstructorInfo[] constructors = implementationType.GetConstructors();

            // step 1: check for multiple constructors with same number of arguments
            for (int i = 0; i < constructors.Length; i++)
            {
                for (int j = i; j < constructors.Length - 1; j++)
                {
                    if (constructors[j].GetParameters().Length == constructors[j + 1].GetParameters().Length)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            // step 2: get the constructor with the most parameters
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                int length = parameters.Length;

                if (bestLength < length)
                {
                    bestLength = length;
                    bestParameters = parameters;
                }
            }

            return bestParameters;
        }

        /// <summary>
        /// Try and bind to a primitive type.
        /// </summary>
        private static bool TryBindToPrimitive(Type type, out object defaultType)
        {
            defaultType = null;

            // This list dosn't match the binding list below because 
            // we only check for items we know are not null by default 
            if (type == typeof(object)) defaultType = default;
            if (type == typeof(int)) defaultType = default(int);
            if (type == typeof(uint)) defaultType = default(uint);
            if (type == typeof(bool)) defaultType = default(bool);
            if (type == typeof(char)) defaultType = default(char);
            if (type == typeof(byte)) defaultType = default(byte);
            if (type == typeof(sbyte)) defaultType = default(sbyte);
            if (type == typeof(short)) defaultType = default(short);
            if (type == typeof(ushort)) defaultType = default(ushort);
            if (type == typeof(long)) defaultType = default(long);
            if (type == typeof(ulong)) defaultType = default(ulong);
            if (type == typeof(double)) defaultType = default(double);
            if (type == typeof(Guid)) defaultType = default(Guid);
            if (type == typeof(DateTime)) defaultType = default(DateTime);
            if (type == typeof(TimeSpan)) defaultType = default(TimeSpan);

            return type == typeof(object)
                || type == typeof(string)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(bool)
                || type == typeof(char)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(double)
                || type == typeof(Guid)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Enum)
                || type == typeof(Array)
                || type == typeof(ArrayList);
        }
    }
}
