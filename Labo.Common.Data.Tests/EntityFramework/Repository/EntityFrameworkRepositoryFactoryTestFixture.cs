namespace Labo.Common.Data.Tests.EntityFramework.Repository
{
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Reflection;
    using System.Threading;

    using Labo.Common.Data.EntityFramework;
    using Labo.Common.Data.EntityFramework.Mapping;
    using Labo.Common.Data.EntityFramework.Repository;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class EntityFrameworkRepositoryFactoryTestFixture
    {
        [Test]
        public void CreateRepositoryMustCreateOneObjectContextsForAllItsOwnedEntities()
        {
            ObjectContext objectContext = ((IObjectContextAdapter)new CodeFirstDbContext()).ObjectContext;
            EntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(new EntityMappingResolver());
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext, Assembly.GetExecutingAssembly());
            
            EntityFrameworkRepositoryFactory entityFrameworkRepositoryFactory = new EntityFrameworkRepositoryFactory(entityFrameworkObjectContextManager);

            Assert.AreEqual(0, entityFrameworkRepositoryFactory.ObjectContexts.Count);

            entityFrameworkRepositoryFactory.CreateRepository<Customer>();

            Assert.AreEqual(1, entityFrameworkRepositoryFactory.ObjectContexts.Count);

            entityFrameworkRepositoryFactory.CreateRepository<Product>();

            Assert.AreEqual(1, entityFrameworkRepositoryFactory.ObjectContexts.Count);
        }

        [Test]
        public void CreateRepositoryMustBeThreadSafe()
        {
            ObjectContext objectContext = ((IObjectContextAdapter)new CodeFirstDbContext()).ObjectContext;
            EntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(new EntityMappingResolver());
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext, Assembly.GetExecutingAssembly());

            EntityFrameworkRepositoryFactory entityFrameworkRepositoryFactory = new EntityFrameworkRepositoryFactory(entityFrameworkObjectContextManager);

            const int threadCount = 100;
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(
                    () =>
                    {
                        entityFrameworkRepositoryFactory.CreateRepository<Customer>();
                        entityFrameworkRepositoryFactory.CreateRepository<Product>();
                    });
            }

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }

            Assert.AreEqual(1, entityFrameworkRepositoryFactory.ObjectContexts.Count);
        }
    }
}
