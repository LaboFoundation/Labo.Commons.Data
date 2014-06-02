namespace Labo.Common.Data.Tests.Session
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Labo.Common.Data.EntityFramework.Repository;
    using Labo.Common.Data.Repository;
    using Labo.Common.Data.Session;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultSessionTestFixture
    {
        [Test]
        public void GetRepositoryMustReturnSingletonRepositoryInstancePerEntityType()
        {
            IRepositoryFactory repositoryFactory = Substitute.For<IRepositoryFactory>();
            repositoryFactory.CreateRepository<Customer>().Returns(x => new EntityFrameworkRepository<Customer>(null));
            repositoryFactory.CreateRepository<Product>().Returns(x => new EntityFrameworkRepository<Product>(null));

            DefaultSession session = new DefaultSession(repositoryFactory);

            IRepository<Customer> customerRepository1 = session.GetRepository<Customer>();
            IRepository<Customer> customerRepository2 = session.GetRepository<Customer>();

            Assert.AreEqual(customerRepository1, customerRepository2);

            repositoryFactory.Received(1).CreateRepository<Customer>();

            IRepository<Product> productRepository1 = session.GetRepository<Product>();
            IRepository<Product> productRepository2 = session.GetRepository<Product>();

            Assert.AreEqual(productRepository1, productRepository2);

            repositoryFactory.Received(1).CreateRepository<Product>();
        }

        [Test]
        public void GetRepositoryMustBeThreadSafe()
        {
            IRepositoryFactory repositoryFactory = Substitute.For<IRepositoryFactory>();
            repositoryFactory.CreateRepository<Customer>().Returns(x => new EntityFrameworkRepository<Customer>(null));

            DefaultSession session = new DefaultSession(repositoryFactory);

            const int threadCount = 100;
            List<IRepository<Customer>> repositories = new List<IRepository<Customer>>(threadCount);
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(
                    () =>
                        {
                            IRepository<Customer> customerRepository = session.GetRepository<Customer>();
                            lock (repositories)
                            {
                                repositories.Add(customerRepository);
                            }
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

            for (int i = 0; i < threadCount; i++)
            {
                for (int j = i + 1; j < threadCount; j++)
                {
                    Assert.AreSame(repositories[i], repositories[j]);
                }
            }

            repositoryFactory.Received(1).CreateRepository<Customer>();
        }

        [Test]
        public void CommitMustCallSaveChangesMethodsOfAllRepositories()
        {
            IRepositoryFactory repositoryFactory = Substitute.For<IRepositoryFactory>();
            IRepository<Customer> customerRepository = Substitute.For<IRepository<Customer>>();
            repositoryFactory.CreateRepository<Customer>().Returns(x => customerRepository);

            IRepository<Product> productRepository = Substitute.For<IRepository<Product>>();
            repositoryFactory.CreateRepository<Product>().Returns(x => productRepository);

            DefaultSession session = new DefaultSession(repositoryFactory);
            session.GetRepository<Customer>();
            session.GetRepository<Product>();
            session.Commit();

            customerRepository.Received(1).SaveChanges();
            productRepository.Received(1).SaveChanges();
        }

        [Test]
        public void CommitThrowsObjectDisposedExceptionWhenSessionIsDisposed()
        {
            IRepositoryFactory repositoryFactory = Substitute.For<IRepositoryFactory>();
            DefaultSession session = new DefaultSession(repositoryFactory);
            session.Dispose();
            Assert.Throws<ObjectDisposedException>(session.Commit);
        }

        [Test]
        public void DisposeMustCallDisposeMethodsOfAllRepositories()
        {
            IRepositoryFactory repositoryFactory = Substitute.For<IRepositoryFactory>();
            IRepository<Customer> customerRepository = Substitute.For<IRepository<Customer>>();
            repositoryFactory.CreateRepository<Customer>().Returns(x => customerRepository);

            IRepository<Product> productRepository = Substitute.For<IRepository<Product>>();
            repositoryFactory.CreateRepository<Product>().Returns(x => productRepository);

            DefaultSession session = new DefaultSession(repositoryFactory);
            session.GetRepository<Customer>();
            session.GetRepository<Product>();
            session.Dispose();

            customerRepository.Received(1).Dispose();
            productRepository.Received(1).Dispose();
        }
    }
}
