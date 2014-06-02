// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityMappingResolver.cs" company="Labo">
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
//   The EntityMappingResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.EntityFramework.Mapping
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Objects;
    using System.Reflection;

    /// <summary>
    /// The EntityMappingResolver interface.
    /// </summary>
    public interface IEntityMappingResolver
    {
        /// <summary>
        /// Gets the entity mappings using the specified <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="entityAssemblies">The entity assemblies.</param>
        /// <returns>
        /// The list of entity mappings.
        /// </returns>
        IList<EntityMapping> GetEntityMappings(ObjectContext context, params Assembly[] entityAssemblies);

        /// <summary>
        /// Gets the entity mappings using the specified <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entityAssemblies">The entity assemblies.</param>
        /// <returns>The list of entity mappings.</returns>
        IList<EntityMapping> GetEntityMappings(DbContext dbContext, params Assembly[] entityAssemblies);
    }
}