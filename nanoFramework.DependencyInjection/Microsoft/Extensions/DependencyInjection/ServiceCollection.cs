//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System.Collections;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Default implementation of <see cref="IServiceCollection"/>.
    /// </summary>
    public class ServiceCollection : IServiceCollection
    {
        private readonly object _syncLock = new();
        private readonly ArrayList _descriptors = [];

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public int Add(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                return _descriptors.Add(item);
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            lock (_syncLock)
            {
                _descriptors.Clear();
            }
        }

        /// <inheritdoc/>
        public bool Contains(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                return _descriptors.Contains(item);
            }
        }

        /// <inheritdoc/>
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            lock (_syncLock)
            {
                _descriptors.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc/>
        public void Remove(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                _descriptors.Remove(item);
            }
        }

        /// <inheritdoc/>
        public IEnumerator GetEnumerator()
        {
            lock (_syncLock)
            {
                return _descriptors.GetEnumerator();
            }
        }

        /// <inheritdoc/>
        public int IndexOf(ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                return _descriptors.IndexOf(item);
            }
        }

        /// <inheritdoc/>
        public void Insert(int index, ServiceDescriptor item)
        {
            lock (_syncLock)
            {
                _descriptors.Insert(index, item);
            }
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            lock (_syncLock)
            {
                _descriptors.RemoveAt(index);
            }
        }
    }
}