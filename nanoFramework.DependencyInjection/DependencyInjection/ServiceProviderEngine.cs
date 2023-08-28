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
            for (int index = Services.Count - 1; index >= 0; index--)
            {
                if (Services[index].ImplementationInstance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the last added service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <param name="scopeServices"></param>
        internal object GetService(Type serviceType, IServiceCollection scopeServices = null)
        {
            var services = GetServiceObjects(serviceType, scopeServices);

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
        /// <param name="scopeServices"></param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> can't be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="serviceType"/> can't be empty.</exception>
        internal object[] GetService(Type[] serviceType, IServiceCollection scopeServices = null)
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
                var services = GetServiceObjects(serviceType[0], scopeServices);

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
                var services = GetServiceObjects(type, scopeServices);

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
        /// <param name="scopeServices"></param>
        private object[] GetServiceObjects(Type serviceType, IServiceCollection scopeServices)
        {
            ArrayList services = new ArrayList();

            if (scopeServices != null)
            {
                foreach (ServiceDescriptor descriptor in scopeServices)
                {
                    if (descriptor.ServiceType == serviceType)
                    {
                        descriptor.ImplementationInstance ??= Resolve(descriptor.ImplementationType);
                            services.Add(descriptor.ImplementationInstance);
                    }
                }
            }

            foreach (ServiceDescriptor descriptor in Services)
            {
                if (descriptor.ServiceType == serviceType)
                {
                    switch (descriptor.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            descriptor.ImplementationInstance ??= Resolve(descriptor.ImplementationType);
                            services.Add(descriptor.ImplementationInstance);
                            break;
                        
                        case ServiceLifetime.Transient:
                            services.Add(Resolve(descriptor.ImplementationType));
                            break;

                        case ServiceLifetime.Scope:
                            if (scopeServices == null) //no scope, just behave as Transient
                            {
                                services.Add(Resolve(descriptor.ImplementationType));
                            }
                            break;    
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
                throw new InvalidOperationException(
                    $"Constructor for '{implementationType}' could not be located.");
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

                    if (parameterType.IsResolvable())
                    {
                        types[index] = parameterType;
                        parameters[index] = GetResolvableDefault(parameterType);
                    }
                    else
                    {
                        var service = GetService(parameterType);

                        if (service == null)
                        {
                            throw new InvalidOperationException(
                                $"Unable to resolve service for '{parameterType}'.");
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
                        throw new InvalidOperationException(
                             $"Multiple constructors found in '{implementationType}'.");
                    }
                }
            }

            // step 2: get the constructor with the most resolvable parameters
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                int length = parameters.Length;

                foreach (ParameterInfo parameter in parameters)
                {
                    Type type = parameter.ParameterType;

                    if (type.IsResolvable())
                    {
                        // check for simple binding first
                    }
                    else if (GetService(type) == null)
                    {
                        // binding could not be resolved ingore constructor
                        length = -1;
                    }
                }

                if (bestLength < length)
                {
                    bestLength = length;
                    bestParameters = parameters;
                }
            }

            return bestParameters;
        }

        /// <summary>
        /// Get primitive default type.
        /// </summary>
        private static object GetResolvableDefault(Type type)
        {
            // This list dosn't match the IsResolvable() because 
            // we only check for items we know are not null by default 
            if (type == typeof(object)) return default;
            if (type == typeof(int)) return default(int);
            if (type == typeof(uint)) return default(uint);
            if (type == typeof(bool)) return default(bool);
            if (type == typeof(char)) return default(char);
            if (type == typeof(byte)) return default(byte);
            if (type == typeof(sbyte)) return default(sbyte);
            if (type == typeof(short)) return default(short);
            if (type == typeof(ushort)) return default(ushort);
            if (type == typeof(long)) return default(long);
            if (type == typeof(ulong)) return default(ulong);
            if (type == typeof(double)) return default(double);
            if (type == typeof(float)) return default(float);
            if (type == typeof(Guid)) return default(Guid);
            if (type == typeof(DateTime)) return default(DateTime);
            if (type == typeof(TimeSpan)) return default(TimeSpan);

            return null;
        }
    }
}
