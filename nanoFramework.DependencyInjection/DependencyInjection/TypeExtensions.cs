using System;
using System.Collections;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Contains extension methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Compares this instance to a specified type and returns an indication if resolvable value.
        /// </summary>
        /// <param name="type">The current <see cref="Type"/></param>
        public static bool IsResolvable(this Type type)
        {
            return type == typeof(string)
                || type == typeof(bool)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(object)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || type == typeof(Array)
                || type == typeof(ArrayList);
        }
    }
}
