//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// The <see cref="System.IDisposable.Dispose"/> method ends the scope lifetime. Once Dispose
    /// is called, any scoped services that have been resolved from
    /// <see cref="IServiceProvider"/> will be disposed.
    /// </summary>
    internal sealed class ServiceProviderEngineScope : IServiceScope, IServiceProvider
    {
        private bool _disposed;

        private readonly IServiceCollection _scopeServices = new ServiceCollection();

        /// <summary>
        /// The root service provider used to resolve dependencies from the scope.
        /// </summary>
        internal ServiceProvider RootProvider { get; }
        
        /// <summary>
        /// Creates instance of <see cref="ServiceProviderEngineScope"/>.
        /// </summary>
        /// <param name="provider">The root service provider used to resolve dependencies from the scope.</param>
        internal ServiceProviderEngineScope(ServiceProvider provider)
        {
            RootProvider = provider;

            CloneScopeServices();
        }

        /// <summary>
        /// The <see cref="IServiceProvider"/> resolved from the scope.
        /// </summary>
        public IServiceProvider ServiceProvider => this;

        /// <inheritdoc/>
        public object GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            return RootProvider.Engine.GetService(serviceType, _scopeServices);
        }

        /// <inheritdoc/>
        public object[] GetService(Type[] serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            return RootProvider.Engine.GetService(serviceType, _scopeServices);
        }

        /// <inheritdoc />
        public IServiceScope CreateScope()
        {
            return new ServiceProviderEngineScope(RootProvider);
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
            foreach (ServiceDescriptor descriptor in RootProvider.Engine.Services)
            {
                if (descriptor.Lifetime == ServiceLifetime.Scoped)
                {
                    _scopeServices.Add(new ServiceDescriptor(
                        descriptor.ServiceType, descriptor.ImplementationType, ServiceLifetime.Scoped));
                }
            }
        }

        private void DisposeServices()
        {
            for (int index = _scopeServices.Count - 1; index >= 0; index--)
            {
                if (_scopeServices[index].ImplementationInstance is IDisposable disposable)
                {
#pragma warning disable S3966 //services must be disposed explicitly, otherwise ServiceRegisteredWithScopeIsDisposedWhenScopeIsDisposed test fails
                    disposable.Dispose();
#pragma warning restore S3966
                }
            }
        }
    }
}
