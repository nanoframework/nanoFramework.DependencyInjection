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
                if (descriptor.ServiceType != typeof(IServiceProvider))
                {
                    if (descriptor.ImplementationInstance is IDisposable instance)
                    {
                        instance.Dispose();
                    }
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
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> can't be null.</exception>
        /// <exception cref="ArgumentException"><paramref name="serviceType"/> can't be empty.</exception>
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
                      & descriptor.ImplementationInstance != null)
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

                    // TODO: I can't figure out a better way to bind primitives or create defaults. Any ideas are welcome?
                    if (CanBindPrimitive(parameterType))
                    {
                        types[index] = parameterType;
                        parameters[index] = CreateDefaultPrimitive(parameterType);
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

            // step 1: check for multiple consturctors with same number of arguments
            for (int i = 0; i < constructors.Length; i++)
            {
                for (int j = i; j < constructors.Length - 1; j++)
                {
                    if (constructors[j].GetParameters().Length
                        == constructors[j + 1].GetParameters().Length)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            // setp 2: get the constructor with the most parameters
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
        /// Attempts to bind to a primitive.
        /// </summary>
        private static bool CanBindPrimitive(Type type)
        {
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
                || type == typeof(object[])
                || type == typeof(string[])
                || type == typeof(int[])
                || type == typeof(uint[])
                || type == typeof(bool[])
                || type == typeof(char[])
                || type == typeof(byte[])
                || type == typeof(sbyte[])
                || type == typeof(short[])
                || type == typeof(ushort[])
                || type == typeof(long[])
                || type == typeof(ulong[])
                || type == typeof(double[])
                || type == typeof(Guid)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Enum)
                || type == typeof(Array)
                || type == typeof(ArrayList);
        }

        /// <summary>
        /// Bind to a primitive using default values.
        /// </summary>
        private static object CreateDefaultPrimitive(Type type)
        {
            if (type == typeof(object)) return default;
            if (type == typeof(string)) return default(string);
            if (type == typeof(int)) return default(int);
            if (type == typeof(uint)) return default(uint);
            if (type == typeof(bool)) return default(bool);
            if (type == typeof(bool)) return default(char);
            if (type == typeof(byte)) return default(byte);
            if (type == typeof(sbyte)) return default(sbyte);
            if (type == typeof(short)) return default(short);
            if (type == typeof(ushort)) return default(ushort);
            if (type == typeof(long)) return default(long);
            if (type == typeof(ulong)) return default(ulong);
            if (type == typeof(double)) return default(double);
            if (type == typeof(object[])) return default(object[]);
            if (type == typeof(string[])) return default(string[]);
            if (type == typeof(int[])) return default(int[]);
            if (type == typeof(uint[])) return default(uint[]);
            if (type == typeof(bool[])) return default(bool[]);
            if (type == typeof(char[])) return default(char[]);
            if (type == typeof(byte[])) return default(byte[]);
            if (type == typeof(sbyte[])) return default(sbyte[]);
            if (type == typeof(short[])) return default(short[]);
            if (type == typeof(ushort[])) return default(ushort[]);
            if (type == typeof(long[])) return default(long[]);
            if (type == typeof(ulong[])) return default(ulong[]);
            if (type == typeof(double[])) return default(double[]);
            if (type == typeof(Guid)) return default(Guid);
            if (type == typeof(DateTime)) return default(DateTime);
            if (type == typeof(TimeSpan)) return default(TimeSpan);
            if (type == typeof(Enum)) return default(Enum);
            if (type == typeof(Array)) return default(Array);
            if (type == typeof(ArrayList)) return default(ArrayList);

            return null;
        }
    }
}