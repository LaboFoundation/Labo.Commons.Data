namespace Labo.Common.Data.Tests.EntityFramework.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Linq;

    using Labo.Common.Data.EntityFramework.Repository;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class EntityFrameworkRepositoryTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            using (ObjectContext objectContext = CreateObjectContext())
            {
                using (EntityFrameworkRepository<OrderItem> entityFrameworkRepository = new EntityFrameworkRepository<OrderItem>(objectContext))
                {
                    DeleteAllOrderItems(entityFrameworkRepository);
                }

                using (EntityFrameworkRepository<Order> entityFrameworkRepository = new EntityFrameworkRepository<Order>(objectContext))
                {
                    DeleteAllOrders(entityFrameworkRepository);
                }

                using (EntityFrameworkRepository<Product> entityFrameworkRepository = new EntityFrameworkRepository<Product>(objectContext))
                {
                    DeleteAllProducts(entityFrameworkRepository);
                }

                using (EntityFrameworkRepository<Customer> entityFrameworkRepository = new EntityFrameworkRepository<Customer>(objectContext))
                {
                    DeleteAllCustomers(entityFrameworkRepository);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Insert()
        {
            string customerName = Guid.NewGuid().ToString();
            const string productName = "Apple";
            const int quantity = 2;

            using (EntityFrameworkRepository<Customer> entityFrameworkRepository = CreateRepository<Customer>(CreateObjectContext()))
            {
                entityFrameworkRepository.Insert(
                    new Customer
                    {
                        Name = customerName,
                        Orders = new List<Order>
                                 {
                                    new Order
                                        {
                                            CreateDate = DateTime.Now,
                                            OrderItems = new List<OrderItem>
                                                         {
                                                            new OrderItem
                                                                {
                                                                    Product = new Product
                                                                            {
                                                                                Name = productName
                                                                            },
                                                                    Quantity = quantity
                                                                }
                                                         }
                                        }
                                }
                    });
                Assert.IsNull(entityFrameworkRepository.Query().SingleOrDefault(x => x.Name == customerName));

                entityFrameworkRepository.SaveChanges();
            }

            using (EntityFrameworkRepository<Customer> customerRepository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer =
                    customerRepository.Query()
                        .Include(x => x.Orders)
                        .Include(x => x.Orders.Select(y => y.OrderItems))
                        .Include(x => x.Orders.Select(y => y.OrderItems.Select(z => z.Product)))
                        .SingleOrDefault(x => x.Name == customerName);

                Assert.AreEqual(customerName, customer.Name);

                using (EntityFrameworkRepository<Order> orderRepository = CreateRepository<Order>(CreateObjectContext()))
                {
                    Order order = orderRepository.Query().SingleOrDefault(x => x.CustomerId == customer.Id);
                    Assert.IsNotNull(order);
                }
            }
        }

        private static EntityFrameworkRepository<TEntity> CreateRepository<TEntity>(ObjectContext objectContext) where TEntity : class
        {
            return new EntityFrameworkRepository<TEntity>(objectContext);
        }

        private static ObjectContext CreateObjectContext()
        {
            ObjectContext objectContext = ((IObjectContextAdapter)new CodeFirstDbContext("EfRepositoryDbContext")).ObjectContext;
            objectContext.ContextOptions.ProxyCreationEnabled = objectContext.ContextOptions.LazyLoadingEnabled = false;
            return objectContext;
        }

        private static void DeleteAllOrderItems(EntityFrameworkRepository<OrderItem> entityFrameworkRepository)
        {
            IList<OrderItem> orderItems = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < orderItems.Count; i++)
            {
                entityFrameworkRepository.Delete(orderItems[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }

        private static void DeleteAllOrders(EntityFrameworkRepository<Order> entityFrameworkRepository)
        {
            IList<Order> orders = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < orders.Count; i++)
            {
                entityFrameworkRepository.Delete(orders[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }

        private static void DeleteAllProducts(EntityFrameworkRepository<Product> entityFrameworkRepository)
        {
            IList<Product> products = entityFrameworkRepository.LoadAll();
            for (int i = 0; i < products.Count; i++)
            {
                entityFrameworkRepository.Delete(products[i]);
            }

            entityFrameworkRepository.SaveChanges();
        }

        private static void DeleteAllCustomers(EntityFrameworkRepository<Customer> entityFrameworkRepository)
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
