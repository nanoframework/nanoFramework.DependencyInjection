//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System.Collections;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Default implementation of IServiceCollection.
    /// </summary>
    public interface IServiceCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="ServiceDescriptor"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to add.</param>
        ServiceDescriptor this[int index] { get; set; }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The <see cref="ServiceDescriptor"/> to add.</param>
        int Add(ServiceDescriptor item);

        /// <summary>
        /// Removes all <see cref="ServiceDescriptor"/> from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="ServiceDescriptor"/> to locate in the collection.</param>
        bool Contains(ServiceDescriptor item);

        /// <summary>
        /// Copies the elements of the collection to an Array starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void CopyTo(ServiceDescriptor[] array, int arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator GetEnumerator();

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The <see cref="ServiceDescriptor"/> to get the index of.</param>
        int IndexOf(ServiceDescriptor item);

        /// <summary>
        /// Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="ServiceDescriptor"/> should be inserted.</param>
        /// <param name="item">The <see cref="ServiceDescriptor"/> to insert.</param>
        void Insert(int index, ServiceDescriptor item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The <see cref="ServiceDescriptor"/> to remove.</param>
        void Remove(ServiceDescriptor item);

        /// <summary>
        /// Removes the <see cref="ServiceDescriptor"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void RemoveAt(int index);
    }
}
