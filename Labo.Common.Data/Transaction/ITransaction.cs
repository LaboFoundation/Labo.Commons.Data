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
    }
}
