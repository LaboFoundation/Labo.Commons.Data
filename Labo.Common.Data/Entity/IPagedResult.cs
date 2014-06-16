// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPagedResult.cs" company="Labo">
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
//   Defines the IPagedResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Entity
{
    using System;
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IPagedResult<TEntity> : IList<TEntity>, ICloneable
    {
        /// <summary>
        /// Gets the total results count.
        /// </summary>
        int TotalResults { get; }

        /// <summary>
        /// Gets the page number
        /// </summary>
        int Page { get; }

        /// <summary>
        /// Gets the total pages count.
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// Gets the items per page.
        /// </summary>
        int ItemsPerPage { get; }

        /// <summary>
        /// Gets the index of the first item.
        /// </summary>
        /// <value>
        /// The start index of the first item.
        /// </value>
        int FirstItemIndex { get; }

        /// <summary>
        /// Gets the index of the last item.
        /// </summary>
        /// <value>
        /// The index of the last item.
        /// </value>
        int LastItemIndex { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        IList<TEntity> Items { get; } 
    }
}