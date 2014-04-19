namespace Labo.Common.Data.Session
{
    using System;
    using System.Threading;

    using Labo.Common.Data.Repository;
    using Labo.Common.Data.Resources;

    public sealed class SessionScope : ISessionScope
    {
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
        /// The session
        /// </summary>
        private ISession m_Session;

        private bool m_HasCommitableSession;

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
                return m_Session;
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
                m_Session = sessionFactory.CreateSession();
            }
            else
            {
                m_HasCommitableSession = false;
                m_Session = Current.Session;
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

        public void Commit()
        {
        }

        public void Rollback()
        {
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
                        m_Disposed = true;
                        return;
                    }

                    Rollback();
                }
                finally
                {
                    m_Disposed = true;
                    s_Head = m_Parent;

                    if (m_Parent == null)
                    {
                        m_Session.Dispose();
                        m_Session = null;
                    }

                    Thread.EndThreadAffinity();
                }
            }
        }
    }
}