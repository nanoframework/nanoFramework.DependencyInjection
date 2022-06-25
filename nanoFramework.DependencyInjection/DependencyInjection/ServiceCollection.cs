// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace nanoFramework.DependencyInjection
{
    /// <summary>
    /// Default implementation of <see cref="IServiceCollection"/>.
    /// </summary>
    public class ServiceCollection : IServiceCollection
    {
        private static readonly object _syncLock = new object();
        private readonly ArrayList _descriptors = new ArrayList();

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (_syncLock)
                {
                    return _descriptors.Count;
                }
            }
        }

        /// <inheritdoc />
        public ServiceDescriptor this[int index]
        {
            get
            {
                lock (_syncLock)
                {
                    return (ServiceDescriptor)_descriptors[index];
                }
            }
            set
            {
                lock (_syncLock)
                {
                    _descriptors[index] = value;
                }
            }
        }

        /// <inheritdoc />
        public int Add(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                return _descriptors.Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_syncLock)
            {
                _descriptors.Clear();
            }
        }

        /// <inheritdoc />
        public bool Contains(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                return _descriptors.Contains(item);
            }
        }

        /// <inheritdoc />
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            lock (_syncLock)
            {
                _descriptors.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public void Remove(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                _descriptors.Remove(item);
            }
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            lock (_syncLock)
            {
                return _descriptors.GetEnumerator();
            }
        }

        /// <inheritdoc />
        public int IndexOf(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                return _descriptors.IndexOf(item);
            }
        }

        /// <inheritdoc />
        public void Insert(int index, ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                _descriptors.Insert(index, item);
            }
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            lock (_syncLock)
            {
                _descriptors.RemoveAt(index);
            }
        }
    }
}