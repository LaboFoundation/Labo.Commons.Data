// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransaction.cs" company="Labo">
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
//   Defines the ITransaction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Transaction
{
    using System;
    using System.Data;

    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Begins the transaction with the default isolation level.
        /// </summary>
        void Begin();

        /// <summary>
        /// Begins the transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">Isolation level of the transaction</param>
        void Begin(IsolationLevel isolationLevel);

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Forces the underlying transaction to roll back.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Gets a value indicating whether the transaction [is active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is active]; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value indicating whether the transaction [was rolled back].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was rolled back]; otherwise, <c>false</c>.
        /// </value>
        bool WasRolledBack { get; }

        /// <summary>
        /// Gets a value indicating whether the transaction [was committed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was committed]; otherwise, <c>false</c>.
        /// </value>
        bool WasCommitted { get; }

        IsolationLevel IsolationLevel { get; }
    }
}
