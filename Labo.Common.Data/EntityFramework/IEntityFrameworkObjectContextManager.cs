namespace Labo.Common.Data.EntityFramework
{
    using System;
    using System.Data.Objects;

    /// <summary>
    /// The EntityFrameworkObjectContextManager interface.
    /// </summary>
    public interface IEntityFrameworkObjectContextManager
    {
        /// <summary>
        /// Gets the object context for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>The object context.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ObjectContext GetObjectContext<TEntity>();

        /// <summary>
        /// Gets the object context.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The object context.</returns>
        ObjectContext GetObjectContext(Type type);

        /// <summary>
        /// Gets the table name that is mapped to the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The table name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        string GetTableName<TEntity>();

        /// <summary>
        /// Gets the name of the table for the specified entity type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The table name.</returns>
        string GetTableName(Type type);

        /// <summary>
        /// Registers the object context creator.
        /// </summary>
        /// <param name="contextProvider">The context provider.</param>
        void RegisterObjectContextCreator(Func<ObjectContext> contextProvider);
    }
}