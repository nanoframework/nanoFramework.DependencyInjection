using System;

namespace nanoFramework.DependencyInjection.System
{
    /// <summary>
    /// Optional service used to determine if the specified type is available from the <see cref="IServiceProvider"/>.
    /// </summary>
    public interface IServiceProviderIsService
    {
        /// <summary>
        /// Determines if the specified service type is available from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to test.</param>
        /// <returns>true if the specified service is a available, false if it is not.</returns>
        bool IsService(Type serviceType);
    }
}
