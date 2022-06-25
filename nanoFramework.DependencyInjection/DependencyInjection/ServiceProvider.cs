// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// The default <see cref="IServiceProvider"/>.
    /// </summary>
    public sealed class ServiceProvider : IServiceProvider, IDisposable
    {
        internal ServiceProviderEngine _engine;

        internal ServiceProvider(IServiceCollection services, ServiceProviderOptions options)
        {
            _engine = GetEngine();
            _engine.Services = services;
            _engine.Services.Add(new ServiceDescriptor(typeof(IServiceProvider), this));

            if (options.ValidateOnBuild)
            {
                ArrayList exceptions = null;
                foreach (ServiceDescriptor descriptor in services)
                {
                    try
                    {
                        _engine.ValidateService(descriptor);
                    }
                    catch (Exception ex)
                    {
                        exceptions ??= new ArrayList();
                        exceptions.Add(ex);
                    }
                }

                if (exceptions != null)
                {
                    throw new AggregateException("Some services are not able to be constructed.", exceptions);
                }
            }
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return _engine.GetService(serviceType);
        }

        /// <inheritdoc />
        public object[] GetService(Type[] serviceType)
        {
            return _engine.GetService(serviceType);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _engine.DisposeServices();
        }

        private ServiceProviderEngine GetEngine()
        {
            return ServiceProviderEngine.Instance;
        }
    }
}