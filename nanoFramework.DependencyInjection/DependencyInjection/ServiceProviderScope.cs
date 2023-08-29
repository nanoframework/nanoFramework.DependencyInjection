//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Default implementation of <see cref="IServiceProviderScope"/>.
    /// </summary>
    internal sealed class ServiceProviderScope : IServiceProviderScope
    {
        private bool _disposed;
        private readonly ServiceProvider _rootProvider;

        private readonly IServiceCollection _scopeServices = new ServiceCollection();

        /// <summary>
        /// Creates instance of <see cref="ServiceProviderScope"/>.
        /// </summary>
        /// <param name="rootProvider"></param>
        internal ServiceProviderScope(ServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;

            CloneScopeServices();
        }


        /// <inheritdoc/>
        public object GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            return _rootProvider._engine.GetService(serviceType, _scopeServices);
        }

        /// <inheritdoc/>
        public object[] GetService(Type[] serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            return _rootProvider._engine.GetService(serviceType, _scopeServices);
        }

        /// <inheritdoc />
        public IServiceProviderScope CreateScope()
        {
            return new ServiceProviderScope(_rootProvider);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true; 
            DisposeServices();
        }

        private void CloneScopeServices()
        {
            foreach (ServiceDescriptor descriptor in _rootProvider._engine.Services)
            {
                if (descriptor.Lifetime == ServiceLifetime.Scoped)
                {
                    _scopeServices.Add(new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationType,
                        ServiceLifetime.Scoped));
                }
            }
        }

        private void DisposeServices()
        {
            for (int index = _scopeServices.Count - 1; index >= 0; index--)
            {
                if (_scopeServices[index].ImplementationInstance is IDisposable disposable)
                {
#pragma warning disable S3966
                    //services must be disposed explicitly, otherwise ServiceRegisteredWithScopeIsDisposedWhenScopeIsDisposed test fails
                    disposable.Dispose();
#pragma warning restore S3966
                }
            }
        }
    }
}
