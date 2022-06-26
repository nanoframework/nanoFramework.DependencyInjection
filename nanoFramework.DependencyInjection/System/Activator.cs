// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    /// <summary>
    /// Contains methods to create types of objects locally. This class cannot be inherited.
    /// </summary>
    public static class Activator
    {
        /// <summary>
        /// Creates an instance of the type whose name is specified, using the named assembly.
        /// </summary>
        /// <param name="typename">The fully qualified name of the type to create an instance of.</param>
        public static object CreateInstance(string typename)
        {
            Type type = Type.GetType(typename);

            if (type != null)
            {
                return CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of the specified type using that type's parameterless constructor.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        public static object CreateInstance(Type type)
        {
            return CreateInstance(type, new Type[] { }, new object[] { });
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor that best matches the specified parameters.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. 
        /// If args is an empty array or null, the constructor that takes no parameters (the parameterless constructor) is invoked.</param>
        public static object CreateInstance(Type type, params object[] args)
        {
            Type[] types = args != null ? new Type[args.Length] : new Type[] { };

            for (int index = types.Length - 1; index >= 0; index--)
            {
                types[index] = args[index]?.GetType();
            }

            return CreateInstance(type, types, args);
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor that best matches the specified parameters.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="types">An array of Type objects representing the number, order, and type of the parameters for the desired constructor.
        /// If types is an empty array or null, to get constructor that takes no parameters.</param>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. 
        /// If args is an empty array or null, the constructor that takes no parameters (the parameterless constructor) is invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> cn't be null</exception>
        public static object CreateInstance(Type type, Type[] types, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            
            if (types == null)
            {
                types = new Type[] { };
            }

            if (args == null)
            {
                args = new object[] { };
            }

            return type.GetConstructor(types).Invoke(args);
        }
    }
}