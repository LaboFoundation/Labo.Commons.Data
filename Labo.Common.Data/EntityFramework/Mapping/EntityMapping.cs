namespace Labo.Common.Data.EntityFramework.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    /// <summary>
    /// The entity mapping class.
    /// </summary>
    public sealed class EntityMapping
    {
        /// <summary>
        /// Gets clr type of the entity.
        /// </summary>
        /// <value>
        /// The clr type.
        /// </value>
        public Type ClrType { get; private set; }

        /// <summary>
        /// Gets the entity conceptual type.
        /// </summary>
        /// <value>
        /// The entity conceptual type.
        /// </value>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Gets the conceptual entity set.
        /// </summary>
        /// <value>
        /// The conceptual entity set.
        /// </value>
        public EntitySet EntitySet { get; private set; }

        /// <summary>
        /// Gets the storage entity type.
        /// </summary>
        /// <value>
        /// The storage entity type.
        /// </value>
        public EntityType StoreType { get; private set; }

        /// <summary>
        /// Gets the storage entity set.
        /// </summary>
        /// <value>
        /// The storage entity set.
        /// </value>
        public EntitySet StoreSet { get; private set; }

        /// <summary>
        /// Gets the table name that is mapped to the entity.
        /// </summary>
        /// <value>
        /// The table name that is mapped to the entity.
        /// </value>
        public string TableName { get; private set; }

        /// <summary>
        /// The entity property mappings
        /// </summary>
        private List<PropertyMapping> m_PropertyMappings;

        /// <summary>
        /// Gets the entity property mappings.
        /// </summary>
        /// <value>
        /// The entity property mappings.
        /// </value>
        public IList<PropertyMapping> PropertyMappings
        {
            get
            {
                return m_PropertyMappings ?? (m_PropertyMappings = new List<PropertyMapping>());
            }
        }

        /// <summary>
        /// The entity key mappings
        /// </summary>
        private List<PropertyMapping> m_KeyMappings;

        /// <summary>
        /// Gets the entity key mappings.
        /// </summary>
        /// <value>
        /// The entity key mappings.
        /// </value>
        public IList<PropertyMapping> KeyMappings
        {
            get
            {
                return m_KeyMappings ?? (m_KeyMappings = new List<PropertyMapping>());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapping"/> class.
        /// </summary>
        /// <param name="clrType">Type clr type of the entity.</param>
        /// <param name="entityType">The entity conceptual type.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="storeType">The entity storage type.</param>
        /// <param name="storeSet">The entity store set.</param>
        /// <param name="tableName">The entity storage table name.</param>
        public EntityMapping(Type clrType, EntityType entityType, EntitySet entitySet, EntityType storeType, EntitySet storeSet, string tableName)
        {
            TableName = tableName;
            StoreSet = storeSet;
            StoreType = storeType;
            EntitySet = entitySet;
            EntityType = entityType;
            ClrType = clrType;
        }
    }
}