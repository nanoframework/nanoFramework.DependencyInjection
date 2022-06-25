using System;
using System.Collections;
using System.Reflection;

namespace nanoFramework.DependencyInjection
{
    internal sealed class RuntimeServiceProviderEngine
    {
        internal static RuntimeServiceProviderEngine Instance { get; } = new RuntimeServiceProviderEngine();

        private RuntimeServiceProviderEngine() { }

        internal IServiceCollection ServiceDescriptors { get; set; }

        internal void ValidateService(ServiceDescriptor descriptor)
        {
            GetService(descriptor.GetImplementationType());
        }

        internal void Dispose()
        {
            foreach (ServiceDescriptor descriptor in ServiceDescriptors)
            {
                if (descriptor.ServiceType != typeof(IServiceProvider))
                {
                    if (descriptor.ImplementationInstance is IDisposable instance)
                    {
                        instance.Dispose();
                    }
                }
            }
        }

        internal object GetService(Type serviceType)
        {
            var services = GetServices(serviceType);
            if (services.Length == 0)
            {
                return null;
            }

            // returns the last added service of this type
            return services[services.Length - 1];
        }

        internal object[] GetService(Type[] serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (serviceType.Length == 0)
            {
                throw new ArgumentException(nameof(serviceType));
            }

            // optimized for single item service type
            if (serviceType.Length == 1)
            {
                var services = GetServices(serviceType[0]);
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
                var services = GetServices(type);
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

                for (int index = 0; index < constructorParameters.Length; index++)
                {
                    var parameterType = constructorParameters[index].ParameterType;

                    var service = GetService(parameterType);
                    if (service == null)
                    {
                        throw new InvalidOperationException(
                            $"Unable to resolve service for type '{parameterType}' while attempting to activate.");
                    }

                    parameters[index] = service;
                    types[index] = parameterType;
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

                //TODO: Better way to handel? Selecting the first constructor might not be the best option. 
                return constructor[0].GetParameters();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"A suitable constructor for type '{implementationType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
            }
        }

        private object[] GetServices(Type serviceType)
        {
            ArrayList services = new ArrayList();

            foreach (ServiceDescriptor descriptor in ServiceDescriptors)
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
                                descriptor.ImplementationInstance = instance;
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
    }
}
