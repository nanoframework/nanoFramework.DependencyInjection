//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// The default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <exception cref="AggregateException">Some services are not able to be constructed.</exception>
    public sealed class ServiceProvider : IServiceProvider, IServiceProviderIsService, IDisposable
    {
        private bool _disposed;

        internal ServiceProviderEngine Engine { get; }

        internal ServiceProvider(IServiceCollection services, ServiceProviderOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (options == null)
            {
                throw new ArgumentNullException();
            }

            Engine = GetEngine();
            Engine.Services = services;
            Engine.Services.Add(new ServiceDescriptor(typeof(IServiceProvider), this));
            Engine.Options = options;

            if (options.ValidateOnBuild)
            {
                ArrayList exceptions = null;

                foreach (ServiceDescriptor descriptor in services)
                {
                    try
                    {
                        Engine.ValidateService(descriptor);
                    }
                    catch (Exception ex)
                    {
                        exceptions ??= new ArrayList();
                        exceptions.Add(ex);
                    }
                }

                if (exceptions != null)
                {
                    throw new AggregateException(string.Empty, exceptions);
                }
            }
        }

        /// <inheritdoc/>
        public object GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            return Engine.GetService(serviceType);
        }

        /// <inheritdoc/>
        public object[] GetService(Type[] serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            return Engine.GetService(serviceType);
        }

        /// <inheritdoc />
        public IServiceScope CreateScope()
        {
            return new ServiceProviderEngineScope(this);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Engine.DisposeServices();
        }

        private ServiceProviderEngine GetEngine()
        {
            return ServiceProviderEngine.Instance;
        }

        /// <inheritdoc  />
        public bool IsService(Type serviceType)
        {
            return Engine.IsService(serviceType);
        }
    }
}
