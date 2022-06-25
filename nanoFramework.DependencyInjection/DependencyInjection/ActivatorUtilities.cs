// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Reflection;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Helper code for the various activator services.
    /// </summary>
    public static class ActivatorUtilities
    {
        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="provider">The service provider used to resolve dependencies</param>
        /// <param name="instanceType">The type to activate</param>
        /// <param name="parameters">Constructor arguments not provided by the <paramref name="provider"/>.</param>
        /// <returns>An activated object of type instanceType</returns>
        public static object CreateInstance(IServiceProvider provider, Type instanceType, params object[] parameters)
        {
            int bestLength = -1;

            ConstructorMatcher bestMatcher = default;

            if (!instanceType.IsAbstract)
            {
                foreach (ConstructorInfo constructor in instanceType.GetConstructors())
                {
                    var matcher = new ConstructorMatcher(constructor);
                    int length = matcher.Match(parameters);

                    if (bestLength < length)
                    {
                        bestLength = length;
                        bestMatcher = matcher;
                    }
                }
            }

            if (bestLength == -1)
            {
                string message = $"A suitable constructor for type '{instanceType}' could not be located. Ensure the type is concrete and all parameters of a public constructor are either registered as services or passed as arguments. Also ensure no extraneous arguments are provided.";
                throw new InvalidOperationException(message);
            }

            return bestMatcher.CreateInstance(provider);
        }

        /// <summary>
        /// Retrieve an instance of the given type from the service provider. If one is not found then instantiate it directly.
        /// </summary>
        /// <param name="provider">The service provider</param>
        /// <param name="type">The type of the service</param>
        /// <returns>The resolved service or created instance</returns>
        public static object GetServiceOrCreateInstance(IServiceProvider provider, Type type)
        {
            return provider.GetService(type) ?? CreateInstance(provider, type);
        }

        private struct ConstructorMatcher
        {
            private readonly object[] _parameterValues;
            private readonly ParameterInfo[] _parameters;
            private readonly ConstructorInfo _constructor;

            public ConstructorMatcher(ConstructorInfo constructor)
            {
                _constructor = constructor;
                _parameters = _constructor.GetParameters();
                _parameterValues = new object[_parameters.Length];
            }

            public int Match(object[] givenParameters)
            {
                int applyIndexStart = 0;
                int applyExactLength = 0;
                for (int givenIndex = 0; givenIndex != givenParameters.Length; givenIndex++)
                {
                    Type givenType = givenParameters[givenIndex].GetType();
                    bool givenMatched = false;

                    for (int applyIndex = applyIndexStart; givenMatched == false && applyIndex != _parameters.Length; ++applyIndex)
                    {
                        if (_parameterValues[applyIndex] == null &&
                            _parameters[applyIndex].ParameterType.Equals(givenType))  //TODO: Type.IsAssignableFrom?
                        {
                            givenMatched = true;
                            _parameterValues[applyIndex] = givenParameters[givenIndex];
                            if (applyIndexStart == applyIndex)
                            {
                                applyIndexStart++;
                                if (applyIndex == givenIndex)
                                {
                                    applyExactLength = applyIndex;
                                }
                            }
                        }
                    }

                    if (givenMatched == false)
                    {
                        return -1;
                    }
                }
                return applyExactLength;
            }

            public object CreateInstance(IServiceProvider provider)
            {
                for (int index = 0; index != _parameters.Length; index++)
                {
                    if (_parameterValues[index] == null)
                    {
                        object value = provider.GetService(_parameters[index].ParameterType);
                        if (value == null)
                        {
                            throw new InvalidOperationException($"Unable to resolve service for type '{_parameters[index].ParameterType}' while attempting to activate '{_constructor.DeclaringType}'.");
                        }
                        else
                        {
                            _parameterValues[index] = value;
                        }
                    }
                }

                return _constructor.Invoke(_parameterValues);
            }
        }
    }
}