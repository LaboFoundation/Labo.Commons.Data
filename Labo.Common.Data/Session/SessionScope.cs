// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionScope.cs" company="Labo">
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
//   Defines the SessionScope type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Session
{
    using System;
    using System.Threading;

    using Labo.Common.Data.Repository;
    using Labo.Common.Data.Resources;
    using Labo.Common.Data.Session.Exceptions;

    public sealed class SessionScope : ISessionScope
    {
        internal interface ISessionContainer : ISession
        {
             ISession InnerSession { get; }
        }

        private sealed class SessionContainer : ISessionContainer
        {
            private ISession m_InnerSession;

            private bool m_Aborted;

            private bool m_Disposed;

            public ISession InnerSession
            {
                get { return m_InnerSession; }
            }

            public SessionContainer(ISession session)
            {
                m_InnerSession = session;
            }

            ~SessionContainer()
            {
                Dispose(false);
            }

            public bool Aborted
            {
                get
                {
                    return m_Aborted;
                }
            }

            public void Commit()
            {
                m_InnerSession.Commit();
            }

            public void Abort()
            {
                m_Aborted = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (m_Disposed)
                {
                    return;
                }

                if (disposing)
                {
                    m_InnerSession.Dispose();
                    m_InnerSession = null;
                }
            }
        }

        /// <summary>
        /// The head session scope.
        /// </summary>
        [ThreadStatic]
        private static SessionScope s_Head;

        /// <summary>
        /// The session scope unique identifier
        /// </summary>
        private readonly Guid m_ScopeId;

        /// <summary>
        /// The create date
        /// </summary>
        private readonly DateTime m_CreateDate;

        /// <summary>
        /// The parent Session Scope
        /// </summary>
        private readonly SessionScope m_Parent;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        /// The completed
        /// </summary>
        private bool m_Completed;

        /// <summary>
        /// The session container
        /// </summary>
        private SessionContainer m_SessionContainer;

        private bool m_HasCommitableSession;

        private bool m_SessionAborted;

        /// <summary>
        /// Gets the current session scope.
        /// </summary>
        /// <value>
        /// The current session scope.
        /// </value>
        public static SessionScope Current
        {
            get { return s_Head; }
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ISession Session
        {
            get
            {
                return m_SessionContainer;
            }
        }

        /// <summary>
        /// Gets the scope unique identifier.
        /// </summary>
        /// <value>
        /// The scope unique identifier.
        /// </value>
        public Guid ScopeId
        {
            get { return m_ScopeId; }
        }

        /// <summary>
        /// Gets the create date.
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime CreateDate
        {
            get
            {
                return m_CreateDate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [disposed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disposed]; otherwise, <c>false</c>.
        /// </value>
        internal bool Disposed
        {
            get
            {
                return m_Disposed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [completed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [completed]; otherwise, <c>false</c>.
        /// </value>
        internal bool Completed
        {
            get
            {
                return m_Completed;
            }
        }

        /// <summary>
        /// Gets the parent session scope.
        /// </summary>
        /// <value>
        /// The parent session scope.
        /// </value>
        internal SessionScope Parent
        {
            get
            {
                return m_Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionScope"/> class.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        /// <param name="sessionScopeOption">The session scope option.</param>
        /// <exception cref="System.ArgumentNullException">sessionFactory</exception>
        public SessionScope(ISessionFactory sessionFactory, SessionScopeOption sessionScopeOption = SessionScopeOption.Required)
        {
            if (sessionFactory == null)
            {
                throw new ArgumentNullException("sessionFactory");
            }

            m_ScopeId = Guid.NewGuid();
            m_CreateDate = DateTime.Now;

            Thread.BeginThreadAffinity();

            if (Current == null || sessionScopeOption == SessionScopeOption.RequiresNew)
            {
                m_HasCommitableSession = true;
                m_SessionContainer = new SessionContainer(sessionFactory.CreateSession());
            }
            else
            {
                m_HasCommitableSession = false;
                m_SessionContainer = Current.m_SessionContainer;
            }

            m_Parent = s_Head;
            s_Head = this;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SessionScope"/> class.
        /// </summary>
        ~SessionScope()
        {
            Dispose(false);
        }

        public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Completes the session scope.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">SessionScope</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Complete()
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException("SessionScope");
            }

            if (m_Completed)
            {
                throw new InvalidOperationException(Strings.Current_SessionScope_is_already_completed);
            }

            // If current session is aborted session scope cannot be completed
            if (m_SessionContainer.Aborted)
            {
                throw new SessionAbortedException(Strings.SessionScope_Session_has_aborted);
            }

            m_Completed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    if (m_Completed)
                    {
                        if (m_HasCommitableSession)
                        {
                            // If there is no dependent session scope commit session
                            CommitSession();
                        }
                    }
                    else
                    {
                        Abort();                        
                    }
                }
                finally
                {
                    m_Disposed = true;
                    s_Head = m_Parent;

                    if (m_HasCommitableSession)
                    {
                        m_SessionContainer.Dispose();
                        m_SessionContainer = null;
                    }

                    Thread.EndThreadAffinity();
                }
            }
        }

        private void CommitSession()
        {
            m_SessionContainer.Commit();
        }

        private void Abort()
        {
            m_SessionAborted = true;
            m_SessionContainer.Abort();
        }

        internal bool IsSessionAborted
        {
            get
            {
                return m_SessionContainer == null ? m_SessionAborted : m_SessionContainer.Aborted;
            }
        }
    }
}