namespace Labo.Common.Data.SqlServer.Tests.Repository
{
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;

    using Labo.Common.Data.Repository;
    using Labo.Common.Data.SqlServer.Tests.Data;

    using NUnit.Framework;

    public abstract class BaseEntityFrameworkRepositoryTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            using (ObjectContext objectContext = CreateObjectContext())
            {
                using (IRepository<OrderItem> entityFrameworkRepository = new SqlServerEntityFrameworkRepository<OrderItem>(objectContext, null))
                {
                    DeleteAllOrderItems(entityFrameworkRepository);
                }

                using (IRepository<Order> entityFrameworkRepository = new SqlServerEntityFrameworkRepository<Order>(objectContext, null))
                {
                    DeleteAllOrders(entityFrameworkRepository);
                }

                using (IRepository<Product> entityFrameworkRepository = new SqlServerEntityFrameworkRepository<Product>(objectContext, null))
                {
                    DeleteAllProducts(entityFrameworkRepository);
                }

                using (IRepository<Customer> entityFrameworkRepository = new SqlServerEntityFrameworkRepository<Customer>(objectContext, null))
                {
                    DeleteAllCustomers(entityFrameworkRepository);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        protected abstract IRepository<TEntity> CreateRepository<TEntity>(ObjectContext objectContext)
            where TEntity : class;

        protected abstract ObjectContext CreateObjectContext();

        private static void DeleteAllOrderItems(IRepository<OrderItem> entityFrameworkRepository)
        {
            IList<OrderItem> orderItems = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < orderItems.Count; i++)
            {
                entityFrameworkRepository.Delete(orderItems[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }

        private static void DeleteAllOrders(IRepository<Order> entityFrameworkRepository)
        {
            IList<Order> orders = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < orders.Count; i++)
            {
                entityFrameworkRepository.Delete(orders[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }

        private static void DeleteAllProducts(IRepository<Product> entityFrameworkRepository)
        {
            IList<Product> products = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < products.Count; i++)
            {
                entityFrameworkRepository.Delete(products[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }

        private static void DeleteAllCustomers(IRepository<Customer> entityFrameworkRepository)
        {
            IList<Customer> customers = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < customers.Count; i++)
            {
                entityFrameworkRepository.Delete(customers[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }
    }
}