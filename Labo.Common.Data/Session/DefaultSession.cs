// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSession.cs" company="Labo">
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
//   Defines the DefaultSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Session
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Labo.Common.Data.Repository;

    public sealed class DefaultSession : ISession
    {
        private readonly IRepositoryFactory m_RepositoryFactory;
        private ConcurrentDictionary<Type, object> m_Repositories = new ConcurrentDictionary<Type, object>();

        private bool m_Disposed;

        public DefaultSession(IRepositoryFactory repositoryFactory)
        {
            m_RepositoryFactory = repositoryFactory;
        }

        ~DefaultSession()
        {
            Dispose(false);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            lock (m_Repositories)
            {
                return (IRepository<TEntity>)m_Repositories.GetOrAdd(typeof(TEntity), x => m_RepositoryFactory.CreateRepository<TEntity>());                
            }
        }

        public void Commit()
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException("DefaultSession");
            }

            // TODO: Throw exception if already committed

            IEnumerator<KeyValuePair<Type, object>> repositoriesEnumerator = m_Repositories.GetEnumerator();

            while (repositoriesEnumerator.MoveNext())
            {
                IRepository repository = repositoriesEnumerator.Current.Value as IRepository;
                if (repository != null)
                {
                    repository.SaveChanges();
                }
            }
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

            IEnumerator<KeyValuePair<Type, object>> repositoriesEnumerator = m_Repositories.GetEnumerator();

            while (repositoriesEnumerator.MoveNext())
            {
                IDisposable disposable = repositoriesEnumerator.Current.Value as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            m_Repositories.Clear();
            m_Repositories = null;
            m_Disposed = true;
        }
    }
}
