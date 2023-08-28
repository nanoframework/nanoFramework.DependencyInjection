﻿
using System;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceProviderScope : IServiceProvider, IDisposable
    {
        private bool _disposed;
        private readonly ServiceProvider _rootProvider;

        private readonly IServiceCollection _scopeServices = new ServiceCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootProvider"></param>
        public ServiceProviderScope(ServiceProvider rootProvider)
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
        public IServiceProvider CreateScope()
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
                if (descriptor.Lifetime == ServiceLifetime.Scope)
                {
                    _scopeServices.Add(new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationType,
                        ServiceLifetime.Scope));
                }
            }
        }

        private void DisposeServices()
        {
            for (int index = _scopeServices.Count - 1; index >= 0; index--)
            {
                if (_scopeServices[index].ImplementationInstance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
