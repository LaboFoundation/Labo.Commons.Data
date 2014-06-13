namespace Labo.Common.Data.Tests.Service
{
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    using Labo.Common.Data.EntityFramework.Session;
    using Labo.Common.Data.Session;
    using Labo.Common.Data.SqlServer;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    public sealed class CustomerService
    {
        private readonly ISessionScopeProvider m_SessionScopeProvider;

        public CustomerService()
        {
            BaseEntityFrameworkSessionFactoryProvider entityFrameworkSessionFactoryProvider = new SqlServerEntityFrameworkSessionFactoryProvider();
            entityFrameworkSessionFactoryProvider.ObjectContextManager.RegisterObjectContextCreator(() => ((IObjectContextAdapter)new CodeFirstDbContext()).ObjectContext);
            m_SessionScopeProvider = new SessionScopeProvider(entityFrameworkSessionFactoryProvider);
        }

        public CustomerService(ISessionScopeProvider sessionScopeProvider)
        {
            m_SessionScopeProvider = sessionScopeProvider;
        }

        public Customer GetCustomerByUsername(string username)
        {
            using (ISessionScope sessionScope = m_SessionScopeProvider.CreateSessionScope())
            {
                return sessionScope.GetRepository<Customer>().Query().SingleOrDefault(x => x.Name == username);
            }
        }
    }
}
