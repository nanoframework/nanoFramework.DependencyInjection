// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Reflection;
using System.Collections;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// The default <see cref="IServiceProvider"/>.
    /// </summary>
    public sealed class ServiceProvider : IServiceProvider, IDisposable
    {
        internal IServiceCollection _serviceDescriptors;

        internal ServiceProvider(IServiceCollection descriptors, ServiceProviderOptions options)
        {
            _serviceDescriptors = descriptors;

            if (options.ValidateOnBuild)
            {
                ArrayList exceptions = null;
                foreach (ServiceDescriptor serviceDescriptor in descriptors)
                {
                    try
                    {
                        ValidateService(serviceDescriptor);
                    }
                    catch (Exception ex)
                    {
                        exceptions = exceptions ?? new ArrayList();
                        exceptions.Add(ex);
                    }
                }

                if (exceptions != null)
                {
                    throw new AggregateException("Some services are not able to be constructed", exceptions);
                }
            }

            _serviceDescriptors.Add(new ServiceDescriptor(typeof(IServiceProvider), this));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (ServiceDescriptor descriptor in _serviceDescriptors)
            {
                if (descriptor.ServiceType != typeof(IServiceProvider))
                {
                    if (descriptor.ImplementationInstance is IDisposable instance)
                    {
                        lock (_serviceDescriptors)
                        {
                            instance.Dispose();
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            var services = GetServices(serviceType);
            if (services.Length == 0)
            {
                return null;
            }

            // returns the last added service of this type
            return services[services.Length - 1];
        }

        /// <inheritdoc />
        public object[] GetService(Type[] serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (serviceType.Length == 0)
            {
                throw new ArgumentException(nameof(serviceType));
            }

            if (serviceType.Length == 1)
            {
                var services = GetServices(serviceType[0]);
                if (services.Length > 0)
                {
                    return services;
                }

                return new object[0];
            }

            object[] array = new object[0];
            foreach (Type type in serviceType)
            {
                var services = GetServices(type);
                if (services.Length > 0)
                {
                    var newResponse = new object[array.Length + services.Length];
                    Array.Copy(array, newResponse, array.Length);
                    Array.Copy(services, 0, newResponse, array.Length, services.Length);
                    array = newResponse;
                }
            }

            return array.Length != 0 ? array : new object[0];
        }

        private object[] GetServices(Type serviceType)
        {
            ArrayList services = new ArrayList();

            foreach (ServiceDescriptor descriptor in _serviceDescriptors)
            {
                if (descriptor.ServiceType == serviceType)
                {
                    if (descriptor.Lifetime == ServiceLifetime.Singleton
                      & descriptor.ImplementationInstance != null)
                    {
                        services.Add(descriptor.ImplementationInstance);
                    }
                    else
                    {
                        try
                        {
                            var instance = Resolve(descriptor.ImplementationType);
                            {
                                lock (_serviceDescriptors)
                                {
                                    descriptor.ImplementationInstance = instance;
                                }
                            }

                            services.Add(instance);
                        }
                        catch
                        {
                            throw new InvalidOperationException(
                                    $"Multiple constructors accepting all given argument types have been found in type '{descriptor.ImplementationType}'. There should only be one applicable constructor.");

                        }
                    }
                }
            }

            return (object[])services.ToArray(typeof(object));
        }

        private object Resolve(Type implementationType)
        {
            object instance;

            ParameterInfo[] constructorParameters = GetParameters(implementationType);
            if (constructorParameters.Length == 0)
            {
                instance = Activator.CreateInstance(implementationType);
            }
            else
            {
                Type[] types = new Type[constructorParameters.Length];
                object[] parameters = new object[constructorParameters.Length];

                for (int i = 0; i < constructorParameters.Length; i++)
                {
                    var parameterType = constructorParameters[i].ParameterType;

                    var service = GetService(parameterType);
                    if (service == null)
                    {
                        throw new InvalidOperationException(
                            $"Unable to resolve service for type '{parameterType}' while attempting to activate.");

                    }

                    parameters[i] = service;
                    types[i] = parameterType;
                }

                instance = Activator.CreateInstance(implementationType, types, parameters);
            }

            return instance;
        }

        private ParameterInfo[] GetParameters(Type implementationType)
        {
            try
            {
                ConstructorInfo[] constructor = implementationType.GetConstructors();
                return constructor[0].GetParameters();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"A suitable constructor for type '{implementationType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
            }
        }

        private void ValidateService(ServiceDescriptor descriptor)
        {
            // TODO:  Add service validator
            throw new NotImplementedException();
        }
    }
}