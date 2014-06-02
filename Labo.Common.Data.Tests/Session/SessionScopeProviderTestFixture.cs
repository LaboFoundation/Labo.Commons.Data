namespace Labo.Common.Data.Tests.Session
{
    using Labo.Common.Data.Session;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class SessionScopeProviderTestFixture
    {
        [Test]
        public void CreateSessionScopeMustCallCreateSessionFactoryMethodOfSessionFactoryProviderForEveryCall()
        {
            ISessionFactoryProvider sessionFactoryProvider = Substitute.For<ISessionFactoryProvider>();
            ISessionFactory sessionFactory = Substitute.For<ISessionFactory>();
            sessionFactory.CreateSession().Returns(x => Substitute.For<ISession>());
            sessionFactoryProvider.CreateSessionFactory().Returns(x => sessionFactory);

            SessionScopeProvider sessionScopeProvider = new SessionScopeProvider(sessionFactoryProvider);
            using (sessionScopeProvider.CreateSessionScope())
            {  
            }

            sessionFactoryProvider.Received(1).CreateSessionFactory();

            using (sessionScopeProvider.CreateSessionScope())
            {
            }

            sessionFactoryProvider.Received(2).CreateSessionFactory();
        }
    }
}
