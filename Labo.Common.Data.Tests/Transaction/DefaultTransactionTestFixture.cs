namespace Labo.Common.Data.Tests.Transaction
{
    using System;
    using System.Data;

    using Labo.Common.Data.Transaction;
    using Labo.Common.Data.Transaction.Exceptions;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultTransactionTestFixture
    {
        [Test]
        public void BeginBeginTransactionMustBeCalled()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();

                connection.ReceivedWithAnyArgs(1).BeginTransaction();

                Assert.IsTrue(transaction.IsActive);
            }
        }

        [Test]
        public void BeginThrowsTransactionExceptionWhenConnectionBeginTransactionThrowsException()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            connection.BeginTransaction().Returns(x => { throw new Exception(); });

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                Assert.Throws<TransactionException>(transaction.Begin);
            }
        }

        [Test]
        public void BeginConnectionMustBeOpenedWhenConnectionIsClosed()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);
            connection.State.Returns(x => ConnectionState.Closed);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();

                connection.Received(1).Open();

                Assert.IsTrue(transaction.IsActive);
            }
        }

        [Test]
        public void BeginThrowsTransactionExceptionWhenCalledAfterCommitFails()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);
            dbTransaction.When(x => x.Commit()).Do(x => { throw new Exception(); });

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                Assert.Throws<TransactionException>(transaction.Commit);

                Assert.Throws<TransactionException>(transaction.Begin);
            }
        }

        [Test]
        public void Commit()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                transaction.Commit();

                Assert.IsTrue(transaction.WasCommitted);
                Assert.IsFalse(transaction.IsActive);
                Assert.IsFalse(transaction.WasRolledBack);

                dbTransaction.Received(1).Commit();
            }
        }

        [Test]
        public void CommitThrowsTransactionExceptionWhenDbTransactionCommitThrowsException()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);
            dbTransaction.When(x => x.Commit()).Do(x => { throw new Exception(); });

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                Assert.Throws<TransactionException>(transaction.Commit);
            }
        }

        [Test]
        public void CommitThrowsTransactionExceptionWhenBeginIsNotCalled()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                Assert.Throws<TransactionException>(transaction.Commit);
            }
        }

        [Test]
        public void CommitThrowsTransactionExceptionWhenDbTransactionConnectionNull()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);
            dbTransaction.Connection.Returns(x => null);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                Assert.Throws<TransactionException>(transaction.Commit);
            }
        }

        [Test]
        public void CommitThrowsTransactionExceptionWhenTransactionIsDisposed()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            DefaultTransaction transaction = new DefaultTransaction(connection);
            transaction.Begin();
            transaction.Dispose();
            Assert.Throws<ObjectDisposedException>(transaction.Commit);
        }

        [Test]
        public void Rollback()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                transaction.Rollback();

                Assert.IsFalse(transaction.WasCommitted);
                Assert.IsFalse(transaction.IsActive);
                Assert.IsTrue(transaction.WasRolledBack);

                dbTransaction.Received(1).Rollback();
            }
        }

        [Test]
        public void RollbackThrowsTransactionExceptionWhenBeginIsNotCalled()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                Assert.Throws<TransactionException>(transaction.Rollback);
            }
        }

        [Test]
        public void RollbackThrowsTransactionExceptionWhenDbTransactionConnectionNull()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);
            dbTransaction.Connection.Returns(x => null);

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                Assert.Throws<TransactionException>(transaction.Rollback);
            }
        }

        [Test]
        public void RollbackThrowsTransactionExceptionWhenTransactionIsDisposed()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);

            DefaultTransaction transaction = new DefaultTransaction(connection);
            transaction.Begin();
            transaction.Dispose();
            Assert.Throws<ObjectDisposedException>(transaction.Rollback);
        }

        [Test]
        public void RollbackThrowsTransactionExceptionWhenDbTransactionRollbackThrowsException()
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            connection.BeginTransaction().Returns(x => dbTransaction);
            dbTransaction.When(x => x.Rollback()).Do(x => { throw new Exception(); });

            using (DefaultTransaction transaction = new DefaultTransaction(connection))
            {
                transaction.Begin();
                Assert.Throws<TransactionException>(transaction.Rollback);
            }
        }

        [Test, Sequential]
        public void TransactionLevel(
            [Values(IsolationLevel.Chaos, IsolationLevel.ReadCommitted, IsolationLevel.ReadUncommitted, 
                    IsolationLevel.RepeatableRead, IsolationLevel.Serializable, IsolationLevel.Snapshot)]
            IsolationLevel isolationLevel)
        {
            IDbConnection connection = Substitute.For<IDbConnection>();
           
            IDbTransaction dbTransaction = Substitute.For<IDbTransaction>();
            dbTransaction.IsolationLevel.Returns(isolationLevel);
            connection.BeginTransaction(isolationLevel).Returns(x => dbTransaction);

            IDbTransaction unspecifiedIsolationLevelDbTransaction = Substitute.For<IDbTransaction>();
            unspecifiedIsolationLevelDbTransaction.IsolationLevel.Returns(IsolationLevel.Unspecified);
            connection.BeginTransaction().Returns(x => unspecifiedIsolationLevelDbTransaction);

            DefaultTransaction transaction = new DefaultTransaction(connection);
            transaction.Begin(isolationLevel);

            Assert.AreEqual(isolationLevel, transaction.IsolationLevel);
        }
    }
}
