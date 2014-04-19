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
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        /// <returns>
        /// The list of entity mappings.
        /// </returns>
        IList<EntityMapping> GetEntityMappings(ObjectContext context, Assembly assembly = null);

        /// <summary>
        /// Gets the entity mappings using the specified <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The list of entity mappings.</returns>
        IList<EntityMapping> GetEntityMappings(DbContext dbContext, Assembly assembly = null);
    }
}