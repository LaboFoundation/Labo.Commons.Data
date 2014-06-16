// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagedResult.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the PagedResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Entity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Labo.Common.Data.Resources;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class PagedResult<TEntity> : IPagedResult<TEntity>
    {
        /// <summary>
        /// The results
        /// </summary>
        private readonly IList<TEntity> m_Results;

        /// <summary>
        /// Gets the total results count.
        /// </summary>
        public int TotalResults { get; private set; }

        /// <summary>
        /// Gets the page number
        /// </summary>
        public int Page { get; private set; }

        /// <summary>
        /// Gets the total pages count.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets the items per page.
        /// </summary>
        public int ItemsPerPage { get; private set; }

        /// <summary>
        /// Gets the index of the first item.
        /// </summary>
        /// <value>
        /// The start index of the first item.
        /// </value>
        public int FirstItemIndex { get; private set; }

        /// <summary>
        /// Gets the index of the last item.
        /// </summary>
        /// <value>
        /// The index of the last item.
        /// </value>
        public int LastItemIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{TEntity}"/> class.
        /// </summary>
        public PagedResult()
            : this(new List<TEntity>(0), 0, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{TEntity}"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="totalItemsCount">The total items count.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        public PagedResult(IList<TEntity> results, int totalItemsCount, int pageNumber, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("pageNumber", Strings.PageNo_must_be_greater_than_0);
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", Strings.PageSize_must_be_greater_than_0);
            }

            if (totalItemsCount < 0)
            {
                throw new ArgumentOutOfRangeException("totalItemsCount", Strings.PagedResult_totalItemsCount_cannot_be_smaller_than_0);
            }

            m_Results = results;
            TotalResults = totalItemsCount;
            ItemsPerPage = pageSize;
            Page = pageNumber;
            TotalPages = (int)Math.Ceiling((decimal)totalItemsCount / pageSize);

            CalculateFirstAndLastItemIndexes(pageNumber, pageSize);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{TEntity}"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="totalItemsCount">The total items count.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <param name="pageSize">Size of the page.</param>
        public PagedResult(IList<TEntity> results, int totalItemsCount, int pageNumber, int totalPages, int pageSize = 10)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }

            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("pageNumber", Strings.PageNo_must_be_greater_than_0);
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", Strings.PageSize_must_be_greater_than_0);
            }

            if (totalItemsCount < 0)
            {
                throw new ArgumentOutOfRangeException("totalItemsCount", Strings.PagedResult_totalItemsCount_cannot_be_smaller_than_0);
            }

            if (totalPages < 0)
            {
                throw new ArgumentOutOfRangeException("totalPages", Strings.PagedResult_totalPages_cannot_be_smaller_than_0);
            }

            m_Results = results;
            TotalResults = totalItemsCount;
            ItemsPerPage = pageSize;
            Page = pageNumber;
            TotalPages = totalPages;

            CalculateFirstAndLastItemIndexes(pageNumber, pageSize);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{TEntity}"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public PagedResult(PagedResult<TEntity> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }

            m_Results = results.m_Results;
            TotalResults = results.TotalResults;
            ItemsPerPage = results.ItemsPerPage;
            Page = results.Page;
            TotalPages = results.TotalPages;
            FirstItemIndex = results.FirstItemIndex;
            LastItemIndex = results.LastItemIndex;
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        public int Count
        {
            get { return m_Results.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TEntity> GetEnumerator()
        {
            return m_Results.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public TEntity this[int index]
        {
            get
            {
                return m_Results[index];
            }

            set
            {
                m_Results[index] = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Results.GetEnumerator();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<TEntity> Items
        {
            get
            {
                return m_Results;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(TEntity item)
        {
            return m_Results.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, TEntity item)
        {
            m_Results.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            m_Results.RemoveAt(index);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(TEntity item)
        {
            m_Results.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            m_Results.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(TEntity item)
        {
            return m_Results.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            m_Results.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return m_Results.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(TEntity item)
        {
            return m_Results.Remove(item);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new PagedResult<TEntity>(this);
        }

        /// <summary>
        /// Calculates the first and last item indexes.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        private void CalculateFirstAndLastItemIndexes(int pageNumber, int pageSize)
        {
            FirstItemIndex = (pageNumber - 1) * pageSize;
            LastItemIndex = Math.Min(FirstItemIndex + pageSize, FirstItemIndex + m_Results.Count);
        }
    }
}