// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Reflection;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// The default <see cref="IServiceProvider"/>.
    /// </summary>
    public sealed class ServiceProvider : IDisposable, IServiceProvider
    {
        internal IServiceCollection _serviceDescriptors;
        private bool disposedValue;

        internal ServiceProvider(IServiceCollection serviceDescriptors, ServiceProviderOptions options)
        {
            _serviceDescriptors = serviceDescriptors;

            if (options.ValidateOnBuild)
            {
                ArrayList exceptions = null;
                foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
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
            if (serviceType.Length == 0)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            object[] buffer = new object[0];

            if (serviceType.Length == 1)
            {
                var services = GetServices(serviceType[0]);
                if (services.Length > 0)
                {
                    return services;
                }
            }
            else
            {
                foreach (Type type in serviceType)
                {
                    var services = GetServices(type);
                    if (services.Length > 0)
                    {
                        var newBuffer = new object[buffer.Length + services.Length];
                        Array.Copy(buffer, newBuffer, buffer.Length);
                        Array.Copy(services, 0, newBuffer, buffer.Length, services.Length);
                        buffer = newBuffer;
                    }
                }
            }

            return buffer.Length != 0 ? buffer : new object[0];
        }

        private object[] GetServices(Type serviceType)
        {
            ArrayList services = new ArrayList();

            foreach (ServiceDescriptor serviceDescriptor in _serviceDescriptors)
            {
                if (serviceDescriptor.ServiceType == serviceType)
                {
                    if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton
                      & serviceDescriptor.ImplementationInstance != null)
                    {
                        services.Add(serviceDescriptor.ImplementationInstance);
                    }
                    else
                    {
                        var instance = Resolve(serviceDescriptor.ImplementationType);
                        {
                            lock (_serviceDescriptors)
                            {
                                serviceDescriptor.ImplementationInstance = instance;
                            }
                        }

                        services.Add(instance);
                    }
                }
            }

            //object[] buffer = new object[services.Count];
            //for (int i = 0; i < services.Count; i++)
            //{
            //    buffer[i] = services[i];
            //}

            //return buffer;

            return (object[])services.ToArray(typeof(object));
        }

        private object Resolve(Type implementationType)
        {
            ConstructorInfo constructor = implementationType.GetConstructors()[0];
            ParameterInfo[] constructorParameters = constructor.GetParameters();

            object instance;

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

        private void ValidateService(ServiceDescriptor descriptor)
        {
            // TODO:  Add service validator
            throw new NotImplementedException();
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (ServiceDescriptor descriptor in _serviceDescriptors)
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

                disposedValue = true;
            }
        }

        ~ServiceProvider()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}