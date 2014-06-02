// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityFrameworkObjectContextManager.cs" company="Labo">
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
//   The EntityFrameworkObjectContextManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.EntityFramework
{
    using System;
    using System.Data.Objects;
    using System.Reflection;

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
        /// Gets the object context key for the specified type.
        /// </summary>
        /// <param name="type">The entity framework entity type.</param>
        /// <returns>The object context unique identifier.</returns>
        /// <exception cref="System.ArgumentException">No ObjectContext has been registered for the specified type.</exception>
        Guid GetObjectContextKey(Type type);

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
        /// <param name="entityAssemblies">The entity assemblies.</param>
        void RegisterObjectContextCreator(Func<ObjectContext> contextProvider, params Assembly[] entityAssemblies);
    }
}