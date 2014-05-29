namespace Labo.Common.Data.Tests.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Labo.Common.Data.Session;
    using Labo.Common.Data.Session.Exceptions;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class SessionScopeTestFixture
    {
        [Test]
        public void SessionScopeCurrentMustBeSameInstanceInUsingStatement()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (SessionScope sessionScope = new SessionScope(sessionFactory))
            {
                Assert.AreEqual(sessionScope, SessionScope.Current);
                Assert.AreSame(sessionScope, SessionScope.Current);

                Assert.AreEqual(sessionScope.ScopeId, SessionScope.Current.ScopeId);
                Assert.AreEqual(sessionScope.CreateDate, SessionScope.Current.CreateDate);

                Assert.IsFalse(sessionScope.Disposed);
                Assert.IsFalse(sessionScope.Completed);
            }
        }

        [Test]
        public void SessionScopeCurrentMustBeNullWhenOutOfUsingStatement()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            SessionScope sessionScope = new SessionScope(sessionFactory);
            using (sessionScope)
            {
            }

            Assert.IsNull(SessionScope.Current);

            Assert.IsTrue(sessionScope.Disposed);
            Assert.IsFalse(sessionScope.Completed);
        }

        [Test]
        public void WhenOneSessionScopeIsCreatedThenSessionScopeParentMustBeNull()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (SessionScope sessionScope = new SessionScope(sessionFactory))
            {
                Assert.IsNull(sessionScope.Parent);
            }
        }

        [Test, Sequential]
        public void WhenNestedSessionScopeSessionScopeCurrentMustBeEqualToUsingSessionScope([Values(SessionScopeOption.Required, SessionScopeOption.RequiresNew)]SessionScopeOption sessionScopeOption)
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (SessionScope sessionScope1 = new SessionScope(sessionFactory, sessionScopeOption))
            {
                Assert.AreEqual(sessionScope1, SessionScope.Current);
                Assert.AreSame(sessionScope1, SessionScope.Current);

                using (SessionScope sessionScope2 = new SessionScope(sessionFactory, sessionScopeOption))
                {
                    Assert.AreEqual(sessionScope2, SessionScope.Current);
                    Assert.AreSame(sessionScope2, SessionScope.Current);

                    Assert.IsFalse(sessionScope1.Disposed);
                    Assert.IsFalse(sessionScope1.Completed);

                    Assert.IsFalse(sessionScope2.Disposed);
                    Assert.IsFalse(sessionScope2.Completed);

                    Assert.AreNotEqual(sessionScope1.ScopeId, sessionScope2.ScopeId);
                }

                Assert.AreEqual(sessionScope1, SessionScope.Current);
                Assert.AreSame(sessionScope1, SessionScope.Current);

                Assert.IsFalse(sessionScope1.Disposed);
                Assert.IsFalse(sessionScope1.Completed);
            }
        }

        [Test, Sequential]
        public void WhenNestedSessionScopeChildSessionScopesParentMustBeSamaAsParentSessionScope([Values(SessionScopeOption.Required, SessionScopeOption.RequiresNew)]SessionScopeOption sessionScopeOption)
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (SessionScope sessionScope1 = new SessionScope(sessionFactory, sessionScopeOption))
            {
                using (SessionScope sessionScope2 = new SessionScope(sessionFactory, sessionScopeOption))
                {
                    Assert.AreEqual(sessionScope1, sessionScope2.Parent);
                    Assert.AreSame(sessionScope1, sessionScope2.Parent);

                    Assert.AreEqual(sessionScope1, SessionScope.Current.Parent);
                    Assert.AreSame(sessionScope1, SessionScope.Current.Parent);
                }
            }
        }

        [Test, Sequential]
        public void WhenNestedSessionScopeScopeCurrentMustBeNullWhenOutOfUsingStatement([Values(SessionScopeOption.Required, SessionScopeOption.RequiresNew)]SessionScopeOption sessionScopeOption)
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (new SessionScope(sessionFactory, sessionScopeOption))
            {
                using (new SessionScope(sessionFactory, sessionScopeOption))
                {
                    Assert.IsNotNull(SessionScope.Current);
                }

                Assert.IsNotNull(SessionScope.Current);
            }

            Assert.IsNull(SessionScope.Current);
        }

        [Test, Sequential]
        public void TestNNestedSessionScopes([Values(SessionScopeOption.Required, SessionScopeOption.RequiresNew)]SessionScopeOption sessionScopeOption)
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            Func<SessionScope> createNewSessionScopeFunc = () => new SessionScope(sessionFactory, sessionScopeOption);

            Stack<SessionScope> sessionScopes = new Stack<SessionScope>();

            const int n = 10;
            for (int i = 0; i < n; i++)
            {
                SessionScope sessionScope = createNewSessionScopeFunc();

                Assert.IsNotNull(SessionScope.Current);
                Assert.AreSame(sessionScope, SessionScope.Current);

                sessionScopes.Push(sessionScope);
            }

            while (sessionScopes.Count > 0)
            {
                SessionScope currentSessionScope = sessionScopes.Pop();

                Assert.IsNotNull(SessionScope.Current);
                Assert.AreSame(currentSessionScope, SessionScope.Current);

                currentSessionScope.Dispose();
            }

            Assert.IsNull(SessionScope.Current);
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void CompleteThrowsExceptionWhenSessionScopeIsDisposed()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            SessionScope sessionScope = new SessionScope(sessionFactory);
            using (sessionScope)
            {
            }

            Assert.IsTrue(sessionScope.Disposed);
            
            sessionScope.Complete();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CompleteThrowsExceptionWhenSessionScopeIsAlreadyCompleted()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (SessionScope sessionScope = new SessionScope(sessionFactory))
            {
                Assert.IsFalse(sessionScope.Completed);

                sessionScope.Complete();

                Assert.IsTrue(sessionScope.Completed);

                sessionScope.Complete();
            }
        }

        [Test]
        public void WhenSessionScopeIsCreatedThenSessionFactoryCreateSessionMustBeCalledOnce()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();          
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (new SessionScope(sessionFactory))
            {
                sessionFactory.Received(1).CreateSession();
            }
        }

        [Test]
        public void WhenTwoNestedSessionScopeIsCreatedThenSessionFactoryCreateSessionMustBeCalledOnce()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (new SessionScope(sessionFactory, SessionScopeOption.Required))
            {
                using (new SessionScope(sessionFactory, SessionScopeOption.Required))
                {
                    sessionFactory.Received(1).CreateSession();
                }
            }
        }

        [Test]
        public void WhenTwoNestedSessionScopeIsCreatedWithRequiresNewThenSessionFactoryCreateSessionMustBeCalledTwice()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (new SessionScope(sessionFactory, SessionScopeOption.Required))
            {
                using (new SessionScope(sessionFactory, SessionScopeOption.RequiresNew))
                {
                    sessionFactory.Received(2).CreateSession();
                }
            }
        }

        [Test]
        public void WhenThreeNestedSessionScopeOneOfThemIsCreatedWithRequiresNewThenSessionFactoryCreateSessionMustBeCalledTwice()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            using (new SessionScope(sessionFactory, SessionScopeOption.Required))
            {
                using (new SessionScope(sessionFactory, SessionScopeOption.RequiresNew))
                {
                    using (new SessionScope(sessionFactory, SessionScopeOption.Required))
                    {
                        sessionFactory.Received(2).CreateSession();
                    }
                }
            }
        }

        [Test]
        public void WhenOutOfSessionScopeUsingStatementThenSessionScopeSessionMustBeDisposed()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            bool sessionDisposed = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Dispose()).Do(x => sessionDisposed = true);
            sessionFactory.CreateSession().Returns(session);

            SessionScope sessionScope = new SessionScope(sessionFactory);
            using (sessionScope)
            {
            }

            session.Received(1).Dispose();

            Assert.IsTrue(sessionScope.Disposed);
            Assert.IsNull(sessionScope.Session);
            Assert.IsTrue(sessionDisposed);
        }

        [Test]
        public void WhenTwoNestedSessionScopeCreatedWithRequiredOptionThenSessionScopeSessionDisposeMustBeCalledOnce()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            bool sessionDisposed = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Dispose()).Do(x => sessionDisposed = true);
            sessionFactory.CreateSession().Returns(session);

            using (new SessionScope(sessionFactory))
            {
                using (new SessionScope(sessionFactory, SessionScopeOption.Required))
                {
                }

                session.DidNotReceive().Dispose();
                Assert.IsFalse(sessionDisposed);
            }

            session.Received(1).Dispose();
            Assert.IsTrue(sessionDisposed);
        }

        [Test]
        public void WhenTwoNestedSessionScopeCreatedWithRequiresNewOptionThenSessionScopeSessionDisposeMustBeCalledTwice()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            bool sessionDisposed = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Dispose()).Do(x => sessionDisposed = true);
            sessionFactory.CreateSession().Returns(session);

            using (new SessionScope(sessionFactory, SessionScopeOption.RequiresNew))
            {
                using (new SessionScope(sessionFactory, SessionScopeOption.RequiresNew))
                {
                }

                session.Received(1).Dispose();
                Assert.IsTrue(sessionDisposed);
            }

            session.Received(2).Dispose();
            Assert.IsTrue(sessionDisposed);
        }

        [Test]
        public void SessionScopeSessionMustBeNullWhenOutofUsingStatement()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            SessionScope sessionScope = new SessionScope(sessionFactory);
            using (sessionScope)
            {
            }

            Assert.IsNull(sessionScope.Session);
        }

        [Test]
        public void SessionScopeSessionMustBeEqualToSessionScopeCurrentSessionInTheUsingStatement()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            ISession session = Substitute.For<ISession>();
            sessionFactory.CreateSession().Returns(session);

            using (SessionScope sessionScope = new SessionScope(sessionFactory))
            {
                Assert.AreSame(session, GetInnerSession(sessionScope.Session));
                Assert.AreSame(session, GetInnerSession(SessionScope.Current.Session));
            }
        }

        [Test]
        public void WhenSecondSessionScopeIsCreatedWithRequiredOptionThenItMustUseParentSession()
        {
            ISessionFactory sessionFactory1 = Substitute.For<ISessionFactory>();
            ISession session1 = Substitute.For<ISession>();
            sessionFactory1.CreateSession().Returns(session1);

            ISessionFactory sessionFactory2 = Substitute.For<ISessionFactory>();
            ISession session2 = Substitute.For<ISession>();
            sessionFactory2.CreateSession().Returns(session2);

            using (SessionScope sessionScope = new SessionScope(sessionFactory1, SessionScopeOption.Required))
            {
                Assert.AreSame(session1, GetInnerSession(sessionScope.Session));
                Assert.AreSame(session1, GetInnerSession(SessionScope.Current.Session));

                using (SessionScope sessionScope1 = new SessionScope(sessionFactory2, SessionScopeOption.Required))
                {
                    Assert.AreSame(session1, GetInnerSession(sessionScope1.Session));
                    Assert.AreSame(session1, GetInnerSession(sessionScope.Session));
                    Assert.AreSame(session1, GetInnerSession(SessionScope.Current.Session));

                    using (SessionScope sessionScope2 = new SessionScope(sessionFactory2, SessionScopeOption.RequiresNew))
                    {
                        Assert.AreSame(session2, GetInnerSession(sessionScope2.Session));
                        Assert.AreSame(session2, GetInnerSession(SessionScope.Current.Session));
                    }
                }
            }
        }

        [Test]
        public void WhenSessionScopeCompleteIsCalledThenSessionCommitMustBeCalled()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();

            bool sessionCommitted = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Commit()).Do(x => sessionCommitted = true);

            sessionFactory.CreateSession().Returns(session);

            using (SessionScope sessionScope = new SessionScope(sessionFactory))
            {
                sessionScope.Complete();
            }

            Assert.IsTrue(sessionCommitted);
            session.Received(1).Commit();
            session.Received(1).Dispose();
        }

        [Test]
        public void WhenTwoNestedSessionScopeAreCreatedWithRequiredOptionThenSessionCommitMustBeCalledWhenParentSessionScpoeCommitIsCalled()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();

            bool sessionCommitted = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Commit()).Do(x => sessionCommitted = true);

            sessionFactory.CreateSession().Returns(session);

            using (SessionScope sessionScope1 = new SessionScope(sessionFactory, SessionScopeOption.Required))
            {
                using (SessionScope sessionScope2 = new SessionScope(sessionFactory, SessionScopeOption.Required))
                {
                    sessionScope2.Complete();
                }

                Assert.IsFalse(sessionCommitted);
                session.DidNotReceive().Commit();

                sessionScope1.Complete();
            }

            Assert.IsTrue(sessionCommitted);
            session.Received(1).Commit();
        }

        [Test]
        public void WhenTwoNestedSessionScopeIsCreatedWithRequiredOptionAndInnerSessionScopeIsNotCompletedThenSessionAbortedExceptionMustBeThrownWhenOutterSessionScopeCompleteIsCalled()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();

            bool sessionCommitted = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Commit()).Do(x => sessionCommitted = true);

            sessionFactory.CreateSession().Returns(session);

            Assert.Throws<SessionAbortedException>(
                () =>
                    {
                        using (SessionScope sessionScope1 = new SessionScope(sessionFactory, SessionScopeOption.Required))
                        {
                            using (new SessionScope(sessionFactory, SessionScopeOption.Required))
                            {
                            }

                            Assert.IsFalse(sessionCommitted);
                            session.DidNotReceive().Commit();

                            Assert.IsTrue(sessionScope1.IsSessionAborted);

                            sessionScope1.Complete();
                        }
                    });

            Assert.IsFalse(sessionCommitted);
            session.DidNotReceive().Commit();
        }

        [Test]
        public void WhenSessionScopeCompleteIsNotCalledThenSessionCommitMustNotBeCalled()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();

            bool sessionCommitted = false;
            ISession session = Substitute.For<ISession>();
            session.When(x => x.Commit()).Do(x => sessionCommitted = true);

            sessionFactory.CreateSession().Returns(session);

            using (new SessionScope(sessionFactory))
            {
            }

            Assert.IsFalse(sessionCommitted);
            session.DidNotReceive().Commit();
            session.Received(1).Dispose();
        }

        [Test]
        public void WhenInnerSessionScopeWithRequiresNewOptionAndIsNotCompletedSessionCommitMustNotBeCalled()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();

            Stack<ISession> sessionStack = new Stack<ISession>();
            sessionFactory.CreateSession().Returns(x => sessionStack.Pop());

            bool session1IsCommited = false;
            bool session2IsCommited = false;

            sessionFactory.When(x => x.CreateSession()).Do(
                x =>
                {
                    ISession session = Substitute.For<ISession>();
                    if (sessionStack.Count == 0)
                    {
                        session.When(y => y.Commit()).Do(z => session1IsCommited = true);
                    }
                    else
                    {
                        session.When(y => y.Commit()).Do(z => session2IsCommited = true);
                    }

                    sessionStack.Push(session);
                });

            using (SessionScope sessionScope1 = new SessionScope(sessionFactory, SessionScopeOption.Required))
            {
                SessionScope sessionScope2 = new SessionScope(sessionFactory, SessionScopeOption.RequiresNew);

                using (sessionScope2)
                {
                }

                Assert.IsTrue(sessionScope2.IsSessionAborted);
                Assert.IsFalse(session2IsCommited);

                sessionScope1.Complete();
            }

            Assert.IsTrue(session1IsCommited);
        }

        [Test]
        public void SessionScopeCurrentMustBeUniqueWhenMultiThreaded()
        {
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(Substitute.For<ISession>());

            const int threadCount = 1000;
            List<Guid> scopeIds = new List<Guid>(threadCount);
            List<Thread> threads = new List<Thread>(threadCount);
            List<SessionScope> sessionScopes = new List<SessionScope>(threadCount);

            for (int i = 0; i < threadCount; i++)
            {
                threads.Add(new Thread(
                    x =>
                        {
                            sessionScopes.Add(new SessionScope(sessionFactory, SessionScopeOption.RequiresNew));
                            lock (scopeIds)
                            {
                                scopeIds.Add(SessionScope.Current.ScopeId);                                
                            }
                        }));
            }

            for (int i = 0; i < threads.Count; i++)
            {
                Thread thread = threads[i];
                thread.Start();
            }

            for (int i = 0; i < threads.Count; i++)
            {
                Thread thread = threads[i];
                thread.Join();
            }

            Assert.AreEqual(scopeIds.Count, scopeIds.Distinct().Count());
        }

        private static ISession GetInnerSession(ISession session)
        {
            return ((SessionScope.ISessionContainer)session).InnerSession;
        }
    }
}
