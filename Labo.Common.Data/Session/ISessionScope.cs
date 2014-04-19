namespace Labo.Common.Data.Session
{
    using System;

    using Labo.Common.Data.Repository;

    /// <summary>
    /// The SessionScope interface.
    /// </summary>
    public interface ISessionScope : IDisposable
    {
        /// <summary>
        /// Creates the repository for <see cref="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The repository</returns>
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// Commits all the operations in the scope.
        /// </summary>
        void Commit();
    }
}