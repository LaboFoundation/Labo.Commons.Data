namespace Labo.Common.Data.EntityFramework
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Globalization;

    using Labo.Common.Data.EntityFramework.Exceptions;
    using Labo.Common.Data.EntityFramework.Mapping;

    /// <summary>
    /// The entity framework object context manager.
    /// </summary>
    internal sealed class EntityFrameworkObjectContextManager : IEntityFrameworkObjectContextManager
    {
        /// <summary>
        /// The object context type cache that maps the entity type name to the object context.
        /// </summary>
        private readonly ConcurrentDictionary<Type, Guid> m_ObjectContextEntityTypeCache = new ConcurrentDictionary<Type, Guid>();

        /// <summary>
        /// The object contexts container.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, Func<ObjectContext>> m_ObjectContexts = new ConcurrentDictionary<Guid, Func<ObjectContext>>();
        
        /// <summary>
        /// The entity type to table name mapping container.
        /// </summary>
        private readonly ConcurrentDictionary<Type, string> m_EntityTables = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// The entity mapping resolver
        /// </summary>
        private readonly IEntityMappingResolver m_EntityMappingResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkObjectContextManager"/> class.
        /// </summary>
        /// <param name="entityMappingResolver">The entity mapping resolver.</param>
        public EntityFrameworkObjectContextManager(IEntityMappingResolver entityMappingResolver)
        {
            m_EntityMappingResolver = entityMappingResolver;
        }

        /// <summary>
        /// Gets the object context for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>The object context.</returns>
        public ObjectContext GetObjectContext<TEntity>()
        {
            return GetObjectContext(typeof(TEntity));
        }

        /// <summary>
        /// Gets the object context for the specified entity framework entity type name.
        /// </summary>
        /// <param name="type">Name of the type.</param>
        /// <returns>The object context.</returns>
        public ObjectContext GetObjectContext(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Guid key = GetObjectContextKey(type);
            ObjectContext objectContext = m_ObjectContexts[key]();

            // Disable Proxy Creation
            objectContext.ContextOptions.ProxyCreationEnabled = false;

            // Disable Lazy Loading
            objectContext.ContextOptions.LazyLoadingEnabled = false;
            return objectContext;
        }

        /// <summary>
        /// Gets the table name that is mapped to the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The table name</returns>
        public string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }

        /// <summary>
        /// Gets the name of the table for the specified entity type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The table name.</returns>
        public string GetTableName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string entityTable;
            if (!m_EntityTables.TryGetValue(type, out entityTable))
            {
                throw new NoTableNameRegisteredForTheSpecifiedEntityException(type.FullName);
            }

            return entityTable;
        }

        /// <summary>
        /// Registers the object context creator.
        /// </summary>
        /// <param name="contextProvider">The context provider.</param>
        public void RegisterObjectContextCreator(Func<ObjectContext> contextProvider)
        {
            if (contextProvider == null)
            {
                throw new ArgumentNullException("contextProvider");
            }

            Guid key = Guid.NewGuid();
            m_ObjectContexts.TryAdd(key, contextProvider);
            using (ObjectContext context = contextProvider())
            {
                RegisterObjectContextEntities(context, key);
            }
        }

        /// <summary>
        /// Gets the object context key for the specified type.
        /// </summary>
        /// <param name="type">The entity framework entity type.</param>
        /// <returns>The object context unique identifier.</returns>
        /// <exception cref="System.ArgumentException">No ObjectContext has been registered for the specified type.</exception>
        private Guid GetObjectContextKey(Type type)
        {
            Guid key;
            if (!m_ObjectContextEntityTypeCache.TryGetValue(type, out key))
            {
                throw new NoObjectContextRegisteredForTheSpecifiedEntityException(string.Format(CultureInfo.CurrentCulture, "No ObjectContext has been registered for the specified type: {0}.", type));
            }

            return key;
        }

        /// <summary>
        /// Registers the object context entities.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        private void RegisterObjectContextEntities(ObjectContext context, Guid key)
        {
            IList<EntityMapping> entityMappings = m_EntityMappingResolver.GetEntityMappings(context);
            for (int i = 0; i < entityMappings.Count; i++)
            {
                EntityMapping entityMapping = entityMappings[i];
                Guid guid;
                if (m_ObjectContextEntityTypeCache.TryGetValue(entityMapping.ClrType, out guid))
                {
                    throw new EntityAlreadyRegisteredWithAnotherObjectContextException(string.Format(CultureInfo.CurrentCulture, "'{0}' entity is already registered to another object context with guid '{1}'", entityMapping.ClrType, guid));
                }

                m_ObjectContextEntityTypeCache.TryAdd(entityMapping.ClrType, key);
                m_EntityTables.TryAdd(entityMapping.ClrType, entityMapping.TableName);
            }
        }
    }
}