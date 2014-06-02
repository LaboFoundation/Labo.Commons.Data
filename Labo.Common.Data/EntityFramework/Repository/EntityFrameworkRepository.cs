// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkRepository.cs" company="Labo">
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
//   Defines the EntityFrameworkRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.EntityFramework.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Linq.Expressions;

    using Labo.Common.Data.Entity;
    using Labo.Common.Data.Repository;
    using Labo.Common.Data.Transaction;
    using Labo.Common.Utils;

    public sealed class EntityFrameworkRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ObjectSet<TEntity> m_ObjectSet;
        private readonly ObjectContext m_ObjectContext;

        private bool m_Disposed;

        public EntityFrameworkRepository(ObjectContext objectContext)
        {
            m_ObjectContext = objectContext;
        }

        ~EntityFrameworkRepository()
        {
            Dispose(false);
        }

        public IRepositoryQueryable<TEntity> Query()
        {
            return new EntityFrameworkRepositoryQueryable<TEntity>(GetObjectSet());
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            GetObjectSet().AddObject(entity);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectSet<TEntity> objectSet = GetObjectSet();
            ObjectStateEntry entry;
            if (m_ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                if (entry.State == EntityState.Detached)
                {
                    objectSet.AddObject(entity);
                }
            }
            else
            {
                objectSet.AddObject(entity);
            }
        }

        public int Update(Expression<Func<TEntity, TEntity>> updateExpression)
        {
            throw new NotImplementedException();
        }

        public int Update(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            throw new NotImplementedException();
        }

        public void Update(IDirtyPropertyTrackingEntity<TEntity> dirtyPropertyTrackingEntity)
        {
            if (dirtyPropertyTrackingEntity == null)
            {
                throw new ArgumentNullException("dirtyPropertyTrackingEntity");
            }

            if (dirtyPropertyTrackingEntity.IsDirtyTrackingEnabled())
            {
                UpdateProperties(dirtyPropertyTrackingEntity.GetEntity(), dirtyPropertyTrackingEntity.GetDirtyPropertyNames());
                
                dirtyPropertyTrackingEntity.ClearDirtyProperties();
            }
            else
            {
                Update(dirtyPropertyTrackingEntity.GetEntity());
            }
        }

        public void UpdateProperties(TEntity entity, params Expression<Func<TEntity, object>>[] expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            string[] propertyNames = new string[expression.Length];
            for (int i = 0; i < expression.Length; i++)
            {
                Expression<Func<TEntity, object>> exp = expression[i];
                propertyNames[i] = LinqUtils.GetMemberName(exp);
            }

            UpdateProperties(entity, propertyNames);
        }

        private void UpdateProperties(TEntity entity, params string[] propertyNames)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (propertyNames == null)
            {
                throw new ArgumentNullException("propertyNames");
            }

            ObjectSet<TEntity> objectSet = GetObjectSet();
            objectSet.Attach(entity);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string propertyName = propertyNames[i];
                m_ObjectContext.ObjectStateManager.GetObjectStateEntry(entity).SetModifiedProperty(propertyName);
            }
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            GetObjectSet().DeleteObject(entity);
        }

        public int Delete()
        {
            throw new NotImplementedException();
        }

        public int Delete(Expression<Func<TEntity, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> LoadAll()
        {
            return Query().ToList();
        }

        public void SaveChanges()
        {
            m_ObjectContext.SaveChanges();
        }

        public void Attach(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            IEntityWithKey entityWithKey = entity as IEntityWithKey;
            if (entityWithKey != null)
            {
                m_ObjectContext.Attach(entityWithKey);
            }
            else
            {
                GetObjectSet().Attach(entity);
            }

            m_ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
        }

        public void AttachTo(string entitySetName, TEntity entity)
        {
            m_ObjectContext.AttachTo(entitySetName, entity);
        }

        public void DeleteUnAttached(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectSet<TEntity> objectSet = GetObjectSet();
            objectSet.Attach(entity);
            m_ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);
        }

        public int ExecuteStoreCommand(string commandText, params object[] parameters)
        {
            return m_ObjectContext.ExecuteStoreCommand(commandText, parameters);
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public ITransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(string destinationTable, IEnumerable<TEntity> collection)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(IEnumerable<TEntity> collection)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(string destinationTable, IEnumerable<TEntity> collection, IDbConnection connection, IDbTransaction dbTransaction = null)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(IEnumerable<TEntity> collection, IDbConnection connection, IDbTransaction dbTransaction = null)
        {
            throw new NotImplementedException();
        }

        public IDbConnection GetConnection()
        {
            return m_ObjectContext.Connection;
        }

        public ITransaction GetTransaction()
        {
            throw new NotImplementedException();
        }

        private ObjectSet<TEntity> GetObjectSet()
        {
            return m_ObjectSet ?? (m_ObjectSet = m_ObjectContext.CreateObjectSet<TEntity>());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (m_Disposed)
            {
                return;
            }

            m_ObjectSet = null;
            // m_ObjectContext.Dispose();
            // m_ObjectContext = null;
            m_Disposed = true;
        }
    }
}