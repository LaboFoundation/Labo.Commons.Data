// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkRepositoryFactory.cs" company="Labo">
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
//   Defines the EntityFrameworkRepositoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.EntityFramework.Repository
{
    using System;
    using System.Collections.Concurrent;
    using System.Data.Objects;

    using Labo.Common.Data.Repository;

    // TODO: Make disposable and dispose all objectcontexts
    internal sealed class EntityFrameworkRepositoryFactory : IRepositoryFactory
    {
        private readonly IEntityFrameworkObjectContextManager m_ObjectContextManager;
        private readonly ConcurrentDictionary<Guid, ObjectContext> m_ObjectContexts;

        internal ConcurrentDictionary<Guid, ObjectContext> ObjectContexts
        {
            get
            {
                return m_ObjectContexts;
            }
        }

        public EntityFrameworkRepositoryFactory(IEntityFrameworkObjectContextManager objectContextManager)
        {
            m_ObjectContextManager = objectContextManager;
            m_ObjectContexts = new ConcurrentDictionary<Guid, ObjectContext>();
        }

        public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        {
            Guid objectContextKey = m_ObjectContextManager.GetObjectContextKey(typeof(TEntity));
            ObjectContext objectContext;
            lock (m_ObjectContexts)
            {
                objectContext = m_ObjectContexts.GetOrAdd(objectContextKey, x => m_ObjectContextManager.GetObjectContext<TEntity>());
            }

            return new EntityFrameworkRepository<TEntity>(objectContext);
        }
    }
}
