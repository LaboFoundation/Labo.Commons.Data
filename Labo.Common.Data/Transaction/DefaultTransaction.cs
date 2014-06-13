// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTransaction.cs" company="Labo">
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
//   Defines the DefaultTransaction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Transaction
{
    using System;
    using System.Data;

    using Labo.Common.Data.Transaction.Exceptions;

    public sealed class DefaultTransaction : ITransaction
    {
        private readonly IDbConnection m_Connection;
        private IDbTransaction m_DbTransaction;
        private bool m_Committed;
        private bool m_RolledBack;
        private bool m_CommitFailed;
        private bool m_Begun;
        private bool m_Disposed;

        public bool IsActive
        {
            get { return m_Begun && !m_RolledBack && !m_Committed; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return m_DbTransaction.IsolationLevel; }
        }

        public bool WasRolledBack
        {
            get
            {
                return m_RolledBack;
            }
        }

        public bool WasCommitted
        {
            get
            {
                return m_Committed;
            }
        }

        public DefaultTransaction(IDbConnection connection)
        {
            m_Connection = connection;
        }

        public void Begin()
        {
            Begin(IsolationLevel.Unspecified);
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            CheckNotDisposed();

            if (m_CommitFailed)
            {
                throw new TransactionException("Cannot restart transaction after failed commit");
            }

            try
            {
                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                }

                m_DbTransaction = isolationLevel == IsolationLevel.Unspecified ? m_Connection.BeginTransaction() : m_Connection.BeginTransaction(isolationLevel);
                m_Begun = true;
            }
            catch (Exception e)
            {
                throw new TransactionException("Begin failed with exception", e);
            }

            m_Committed = false;
            m_RolledBack = false;
        }

        public void Commit()
        {
            CheckNotDisposed();
            CheckBegun();
            CheckDbTransaction();

            try
            {
                m_DbTransaction.Commit();

                m_Committed = true;
            }
            catch (Exception e)
            {
                m_CommitFailed = true;
                throw new TransactionException("Commit failed with exception", e);
            }
        }

        public void Rollback()
        {
            CheckNotDisposed();
            CheckBegun();
            CheckDbTransaction();

            if (!m_CommitFailed)
            {
                try
                {
                    m_DbTransaction.Rollback();
                    m_RolledBack = true;
                }
                catch (Exception e)
                {
                    throw new TransactionException("Rollback failed with Exception", e);
                }
            }
        }

        private void CheckNotDisposed()
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException("DefaultTransaction");
            }
        }

        private void CheckBegun()
        {
            if (!m_Begun)
            {
                throw new TransactionException("Transaction not successfully started");
            }
        }

        private void CheckDbTransaction()
        {
            if (m_DbTransaction == null || m_DbTransaction.Connection == null)
            {
                throw new TransactionException("Transaction not connected, or was disconnected");
            }
        }

        ~DefaultTransaction()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (m_DbTransaction != null)
                {
                    m_DbTransaction.Dispose();
                    m_DbTransaction = null;
                }

                m_Disposed = true;
            }
        }
    }
}