// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityMappingResolver.cs" company="Labo">
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
//   The entity mapping resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.EntityFramework.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Labo.Common.Reflection;

    /// <summary>
    /// The entity mapping resolver.
    /// </summary>
    internal sealed class EntityMappingResolver : IEntityMappingResolver
    {
        /// <summary>
        /// Gets the entity mappings using the specified <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="entityAssemblies">The entity assemblies.</param>
        /// <returns>The list of entity mappings.</returns>
        public IList<EntityMapping> GetEntityMappings(DbContext dbContext, params Assembly[] entityAssemblies)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException("dbContext");
            }

            return GetEntityMappings(((IObjectContextAdapter)dbContext).ObjectContext, entityAssemblies);
        }

        /// <summary>
        /// Gets the entity mappings using the specified <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityAssemblies">The entity assemblies.</param>
        /// <returns>The list of entity mappings.</returns>
        public IList<EntityMapping> GetEntityMappings(ObjectContext context, params Assembly[] entityAssemblies)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (entityAssemblies == null)
            {
                throw new ArgumentNullException("entityAssemblies");
            }

            ReflectionHelper.CallMethod(context, "EnsureMetadata");

            MetadataWorkspace metadataWorkspace = context.MetadataWorkspace;
            for (int i = 0; i < entityAssemblies.Length; i++)
            {
                Assembly entityAssembly = entityAssemblies[i];
                if (entityAssembly != null)
                {
                    metadataWorkspace.LoadFromAssembly(entityAssembly);
                }
            }

            List<EntityMapping> entityMappings = new List<EntityMapping>();
           
            FillEntityMappings(metadataWorkspace, entityMappings);

            return entityMappings;
        }

        /// <summary>
        /// Fills the entity mappings.
        /// </summary>
        /// <param name="metadataWorkspace">The metadata workspace.</param>
        /// <param name="entityMappings">The entity mappings.</param>
        private static void FillEntityMappings(MetadataWorkspace metadataWorkspace, ICollection<EntityMapping> entityMappings)
        {
            ReadOnlyCollection<EntityType> ospaceEntityTypes = metadataWorkspace.GetItems<EntityType>(DataSpace.OSpace);

            if (ospaceEntityTypes != null && ospaceEntityTypes.Count > 0)
            {
                ObjectItemCollection objectModelItemCollection = (ObjectItemCollection)metadataWorkspace.GetItemCollection(DataSpace.OSpace);

                for (int i = 0; i < ospaceEntityTypes.Count; i++)
                {
                    EntityType entityType = ospaceEntityTypes[i];
                    Type clrType;
                    if (!objectModelItemCollection.TryGetClrType(entityType, out clrType))
                    {
                        continue;
                    }

                    EntitySet entitySet = GetEntitySet(metadataWorkspace, entityType);
                    if (entitySet == null)
                    {
                        continue;
                    }

                    ItemCollection mappingItemCollection = metadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                    EntityMapping entityMapping = GetEntityMapping(mappingItemCollection, entitySet, entityType, clrType);

                    entityMappings.Add(entityMapping);
                }
            }
        }

        /// <summary>
        /// Gets the entity mapping.
        /// </summary>
        /// <param name="mappingItemCollection">The mapping item collection.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="entityType">Type entity type.</param>
        /// <param name="clrType">Type clr type.</param>
        /// <returns>Entity mapping object.</returns>
        private static EntityMapping GetEntityMapping(ItemCollection mappingItemCollection, EntitySet entitySet, EntityType entityType, Type clrType)
        {
            object storageMappingFragment = FindStorageMappingFragment(mappingItemCollection, entitySet);
            EntitySet storeSet = (EntitySet)ReflectionHelper.GetPropertyValue(storageMappingFragment, "TableSet");

            string tableName = storeSet == null ? QuoteTableName(entityType.Name) : GetTableName(storeSet);

            EntityMapping entityMapping = new EntityMapping(
                clrType,
                entityType,
                entitySet,
                storeSet == null ? null : storeSet.ElementType,
                storeSet,
                tableName);

            if (storageMappingFragment != null)
            {
                SetProperties(entityMapping, storageMappingFragment);
            }

            SetKeys(entityMapping, entityType, entityMapping.PropertyMappings);
            return entityMapping;
        }

        /// <summary>
        /// Gets the mapped table name of the entity using the specified <see cref="EntitySet"/>.
        /// </summary>
        /// <param name="storeSet">The store set.</param>
        /// <returns>The table name.</returns>
        private static string GetTableName(EntitySetBase storeSet)
        {
            StringBuilder builder = new StringBuilder(50);

            string table = null;
            string schema = null;

            MetadataProperty tableProperty;
            MetadataProperty schemaProperty;

            storeSet.MetadataProperties.TryGetValue("Table", true, out tableProperty);
            if (tableProperty == null || tableProperty.Value == null)
            {
                storeSet.MetadataProperties.TryGetValue("http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator:Table", true, out tableProperty);
            }

            if (tableProperty != null)
            {
                table = tableProperty.Value as string;
            }

            // Table will be null if its the same as Name
            if (table == null)
            {
                table = storeSet.Name;
            }

            storeSet.MetadataProperties.TryGetValue("Schema", true, out schemaProperty);
            if (schemaProperty == null || schemaProperty.Value == null)
            {
                storeSet.MetadataProperties.TryGetValue("http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator:Schema", true, out schemaProperty);
            }

            if (schemaProperty != null)
            {
                schema = schemaProperty.Value as string;
            }

            if (!string.IsNullOrWhiteSpace(schema))
            {
                builder.Append(QuoteTableName(schema));
                builder.Append(".");
            }

            builder.Append(QuoteTableName(table));

            return builder.ToString();
        }

        /// <summary>
        /// Quotes the name of the table.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The quoted table name.</returns>
        private static string QuoteTableName(string name)
        {
            return "[" + name.Replace("]", "]]") + "]";
        }

        /// <summary>
        /// Finds the storage mapping fragment.
        /// </summary>
        /// <param name="mappingItemCollection">The mapping item collection.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <returns>The storage mapping fragment</returns>
        private static object FindStorageMappingFragment(ItemCollection mappingItemCollection, EntitySet entitySet)
        {
            // StorageEntityContainerMapping
            GlobalItem storage = mappingItemCollection.FirstOrDefault();
            if (storage == null)
            {
                return null;
            }

            // StorageSetMapping
            IEnumerable<object> mappings = (IEnumerable<object>)ReflectionHelper.GetPropertyValue(storage, "EntitySetMaps");

            foreach (object mapping in mappings)
            {
                EntitySet modelSet = (EntitySet)ReflectionHelper.GetPropertyValue(mapping, "Set");
                if (modelSet == null || modelSet != entitySet)
                {
                    continue;
                }

                // only support first type mapping
                IEnumerable<object> typeMappings = (IEnumerable<object>)ReflectionHelper.GetPropertyValue(mapping, "TypeMappings");

                // StorageEntityTypeMapping
                object typeMapping = typeMappings.FirstOrDefault();
                if (typeMapping == null)
                {
                    continue;
                }

                // only support first mapping fragment
                IEnumerable<object> mappingFragments = (IEnumerable<object>)ReflectionHelper.GetPropertyValue(typeMapping, "MappingFragments");
                if (mappingFragments == null)
                {
                    continue;
                }

                object mappingFragment = mappingFragments.FirstOrDefault();
                if (mappingFragment == null)
                {
                    continue;
                }

                // StorageMappingFragment
                return mappingFragment;
            }

            return null;
        }

        /// <summary>
        /// Gets the entity set from the entity containers by the entity type.
        /// </summary>
        /// <param name="metadataWorkspace">The metadata workspace.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity set.</returns>
        /// <exception cref="System.InvalidOperationException">Multiple entity sets per type is not supported.</exception>
        private static EntitySet GetEntitySet(MetadataWorkspace metadataWorkspace, StructuralType entityType)
        {
            ReadOnlyCollection<EntityContainer> entityContainers = metadataWorkspace.GetItems<EntityContainer>(DataSpace.CSpace);
            if (entityContainers == null)
            {
                return null;
            }

            StructuralType edmSpaceType;

            if (IsClrEntityType(entityType))
            {
                edmSpaceType = entityType;
            }
            else
            {
                if (!metadataWorkspace.TryGetEdmSpaceType(entityType, out edmSpaceType))
                {
                    return null;
                }   
            }

            while (edmSpaceType != null && edmSpaceType.BaseType != null)
            {
                edmSpaceType = (EntityType)edmSpaceType.BaseType;
            }

            Func<EntitySetBase, bool> predicate = x => x.ElementType == edmSpaceType;

            EntitySet entitySet = null;
            for (int i = 0; i < entityContainers.Count; i++)
            {
                EntityContainer container = entityContainers[i];

                List<EntitySetBase> source = container.BaseEntitySets.Where(predicate).ToList();

                int count = source.Count();
                if ((count > 1) || ((count == 1) && (entitySet != null)))
                {
                    throw new InvalidOperationException("Multiple entity sets per type is not supported.");
                }

                if (count == 1)
                {
                    entitySet = (EntitySet)source.First();
                }
            }

            return entitySet;
        }

        /// <summary>
        /// Determines whether [is clr entity type] [the specified entity type].
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns><c>true</c> if the specified entity type [is clr entity type]; otherwise, <c>false</c>.</returns>
        private static bool IsClrEntityType(StructuralType entityType)
        {
            return entityType.GetType().FullName != "System.Data.Metadata.Edm.ClrEntityType";
        }

        /// <summary>
        /// Sets the mapping properties of the entity mapping.
        /// </summary>
        /// <param name="entityMapping">The entity mapping.</param>
        /// <param name="storageMappingFragment">The storage mapping fragment.</param>
        private static void SetProperties(EntityMapping entityMapping, object storageMappingFragment)
        {
            IEnumerable<object> properties = (IEnumerable<object>)ReflectionHelper.GetPropertyValue(storageMappingFragment, "Properties");
            foreach (object property in properties)
            {
                // StorageScalarPropertyMapping
                EdmProperty modelProperty = (EdmProperty)ReflectionHelper.GetPropertyValue(property, "EdmProperty");
                EdmProperty storeProperty = (EdmProperty)ReflectionHelper.GetPropertyValue(property, "ColumnProperty");

                entityMapping.PropertyMappings.Add(new PropertyMapping
                {
                    ColumnName = storeProperty.Name,
                    PropertyName = modelProperty.Name
                });
            }
        }

        /// <summary>
        /// Sets the mapping keys of the entity mapping.
        /// </summary>
        /// <param name="entityMapping">The entity mapping.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityPropertyMappings">The entity property mappings.</param>
        private static void SetKeys(EntityMapping entityMapping, EntityType entityType, IList<PropertyMapping> entityPropertyMappings)
        {
            ReadOnlyMetadataCollection<EdmMember> keyMembers = entityType.KeyMembers;
            for (int i = 0; i < keyMembers.Count; i++)
            {
                EdmMember edmMember = keyMembers[i];
                PropertyMapping property = entityPropertyMappings.FirstOrDefault(p => p.PropertyName == edmMember.Name);
                if (property == null)
                {
                    continue;
                }

                entityMapping.KeyMappings.Add(
                    new PropertyMapping
                        {
                            PropertyName = property.PropertyName, 
                            ColumnName = property.ColumnName
                        });
            }
        }
    }
}
