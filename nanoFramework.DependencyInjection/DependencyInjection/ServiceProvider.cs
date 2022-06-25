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
        internal RuntimeServiceProviderEngine _engine;

        internal ServiceProvider(IServiceCollection descriptors, ServiceProviderOptions options)
        {
            _engine = GetEngine();
            _engine.ServiceDescriptors = descriptors;
            _engine.ServiceDescriptors.Add(new ServiceDescriptor(typeof(IServiceProvider), this));

            if (options.ValidateOnBuild)
            {
                ArrayList exceptions = null;
                foreach (ServiceDescriptor descriptor in descriptors)
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
            _engine.Dispose();
        }

        private RuntimeServiceProviderEngine GetEngine()
        {
            return RuntimeServiceProviderEngine.Instance;
        }
    }
}