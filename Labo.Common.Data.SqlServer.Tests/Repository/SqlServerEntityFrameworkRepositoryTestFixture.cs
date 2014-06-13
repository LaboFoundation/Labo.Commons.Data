namespace Labo.Common.Data.SqlServer.Tests.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Linq;

    using Labo.Common.Data.Repository;
    using Labo.Common.Data.SqlServer;
    using Labo.Common.Data.SqlServer.Tests.Data;

    using NUnit.Framework;

    [TestFixture]
    public class SqlServerEntityFrameworkRepositoryTestFixture : BaseEntityFrameworkRepositoryTestFixture
    {
        [Test]
        public void Insert()
        {
            string customerName = Guid.NewGuid().ToString();
            const string productName = "Apple";
            const int quantity = 2;

            using (IRepository<Customer> entityFrameworkRepository = CreateRepository<Customer>(CreateObjectContext()))
            {
                entityFrameworkRepository.Insert(
                    new Customer
                    {
                        FirstName = customerName,
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
                Assert.IsNull(entityFrameworkRepository.Query().SingleOrDefault(x => x.FirstName == customerName));

                entityFrameworkRepository.SaveChanges();
            }

            using (IRepository<Customer> customerRepository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer =
                    customerRepository.Query()
                        .Include(x => x.Orders)
                        .Include(x => x.Orders.Select(y => y.OrderItems))
                        .Include(x => x.Orders.Select(y => y.OrderItems.Select(z => z.Product)))
                        .SingleOrDefault(x => x.FirstName == customerName);

                Assert.IsNotNull(customer);
                Assert.AreEqual(customerName, customer.FirstName);
                Assert.AreEqual(1, customer.Orders.Count);
                Assert.AreEqual(1, customer.Orders.First().OrderItems.Count);

                OrderItem orderItem = customer.Orders.First().OrderItems.First();
                Assert.AreEqual(quantity, orderItem.Quantity);
                Assert.IsNotNull(orderItem.Product);
                Assert.AreEqual(productName, orderItem.Product.Name);

                using (IRepository<Order> orderRepository = CreateRepository<Order>(CreateObjectContext()))
                {
                    Order order = orderRepository.Query().SingleOrDefault(x => x.CustomerId == customer.Id);
                    Assert.IsNotNull(order);
                }
            }
        }

        [Test]
        public void UpdateAttachedEntity()
        {
            string customerName = Guid.NewGuid().ToString();

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer
                                        {
                                            FirstName = customerName
                                        };
                repository.Insert(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));

                customerName = Guid.NewGuid().ToString();
                customer.FirstName = customerName;

                repository.Update(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }
        }

        [Test]
        public void UpdateAttachedEntity1()
        {
            string customerName = Guid.NewGuid().ToString();

            Customer customer = new Customer
            {
                FirstName = customerName
            };

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                repository.Insert(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                customerName = Guid.NewGuid().ToString();
                int id = customer.Id;
                customer = new Customer
                {
                    Id = id,
                    FirstName = customerName
                };

                repository.Attach(customer);

                repository.Update(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }
        }

        [Test]
        public void UpdateDetachedEntity()
        {
            string customerName = Guid.NewGuid().ToString();

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer
                {
                    FirstName = customerName
                };
                repository.Insert(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                customerName = Guid.NewGuid().ToString();

                Customer customer = new Customer
                {
                    FirstName = customerName
                };

                repository.Update(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }
        }

        [Test]
        public void UpdateDetachedEntity1()
        {
            string customerName = Guid.NewGuid().ToString();

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer
                {
                    FirstName = customerName
                };
                repository.Insert(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));

                repository.Detach(customer);

                customerName = Guid.NewGuid().ToString();
                customer.FirstName = customerName;

                repository.Update(customer);
                repository.SaveChanges();

                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == customerName));
            }
        }

        [Test]
        public void UpdateByExpression()
        {
            const int activeCount = 5;
            const int inactiveCount = 5;
            const int count = activeCount + inactiveCount;

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                for (int i = 0; i < activeCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = true
                    };
                    repository.Insert(customer);
                }

                for (int i = 0; i < inactiveCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = false
                    };
                    repository.Insert(customer);
                }
                
                repository.SaveChanges();

                Assert.AreEqual(count, repository.Query().Count());
                Assert.AreEqual(activeCount, repository.Query().Count(x => x.IsActive));
                Assert.AreEqual(inactiveCount, repository.Query().Count(x => !x.IsActive));

                int updateCount = repository.Update(x => new Customer { IsActive = false });
                Assert.AreEqual(count, updateCount);
                Assert.AreEqual(count, repository.Query().Count(x => !x.IsActive));
            }
        }

        [Test]
        public void UpdateByExpressionWithCriteria()
        {
            const int activeCount = 3;
            const int inactiveCount = 7;
            const int count = activeCount + inactiveCount;

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                for (int i = 0; i < activeCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = true
                    };
                    repository.Insert(customer);
                }

                for (int i = 0; i < inactiveCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = false
                    };
                    repository.Insert(customer);
                }

                repository.SaveChanges();

                Assert.AreEqual(count, repository.Query().Count());
                Assert.AreEqual(activeCount, repository.Query().Count(x => x.IsActive));
                Assert.AreEqual(inactiveCount, repository.Query().Count(x => !x.IsActive));

                int updateCount = repository.Update(x => !x.IsActive, x => new Customer { IsActive = true });
                Assert.AreEqual(inactiveCount, updateCount);
                Assert.AreEqual(count, repository.Query().Count(x => x.IsActive));

                updateCount = repository.Update(x => x.IsActive, x => new Customer { IsActive = false });
                Assert.AreEqual(count, updateCount);
                Assert.AreEqual(count, repository.Query().Count(x => !x.IsActive));
            }
        }

        [Test]
        public void UpdatePropertiesWithAttachedEntity()
        {
            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer();
                customer.FirstName = "1";

                repository.Insert(customer);
                repository.SaveChanges();

                Assert.AreEqual(1, repository.Query().Count());
                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == "1"));

                customer.FirstName = "2";
                customer.LastName = "3";
                repository.UpdateProperties(customer, x => x.FirstName);
                repository.SaveChanges();

                Assert.AreEqual(1, repository.Query().Count());
                Customer updatedCustomer = repository.Query().SingleOrDefault(x => x.FirstName == "2");
                Assert.IsNotNull(updatedCustomer);
                Assert.IsNull(updatedCustomer.LastName);
            }
        }

        [Test]
        public void UpdatePropertiesWithUnAttachedEntity()
        {
            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer();
                customer.FirstName = "1";

                repository.Insert(customer);
                repository.SaveChanges();

                Assert.AreEqual(1, repository.Query().Count());
                Assert.IsNotNull(repository.Query().SingleOrDefault(x => x.FirstName == "1"));

                repository.UpdateProperties(new Customer { Id = customer.Id, FirstName = "2", LastName = "3" }, x => x.FirstName);
                repository.SaveChanges();

                Assert.AreEqual(1, repository.Query().Count());
                Customer updatedCustomer = repository.Query().SingleOrDefault(x => x.FirstName == "2");
                Assert.IsNotNull(updatedCustomer);
                Assert.IsNull(updatedCustomer.LastName);
            }
        }

        [Test]
        public void DeleteAll()
        {
            const int activeCount = 3;
            const int inactiveCount = 7;
            const int count = activeCount + inactiveCount;

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                for (int i = 0; i < activeCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = true
                    };
                    repository.Insert(customer);
                }

                for (int i = 0; i < inactiveCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = false
                    };
                    repository.Insert(customer);
                }

                repository.SaveChanges();

                int deleted = repository.DeleteAll();

                Assert.AreEqual(count, deleted);
                Assert.AreEqual(0, repository.Query().Count());
            }
        }

        [Test]
        public void DeleteByFilter()
        {
            const int activeCount = 3;
            const int inactiveCount = 7;

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                for (int i = 0; i < activeCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = true
                    };
                    repository.Insert(customer);
                }

                for (int i = 0; i < inactiveCount; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = Guid.NewGuid().ToString(),
                        IsActive = false
                    };
                    repository.Insert(customer);
                }

                repository.SaveChanges();

                int deleted = repository.Delete(x => x.IsActive);

                Assert.AreEqual(activeCount, deleted);
                Assert.AreEqual(inactiveCount, repository.Query().Count());
            }
        }

        [Test]
        public void DeleteUnAttached()
        {
            Customer customer = new Customer
            {
                FirstName = Guid.NewGuid().ToString(),
                IsActive = true
            };

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                repository.Insert(customer);

                repository.SaveChanges();
            }

            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                repository.Delete(new Customer { Id = customer.Id });
                repository.SaveChanges();

                Assert.AreEqual(0, repository.Query().Count());
            }
        }

        [Test]
        public void DeleteUnAttached1()
        {
            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer
                {
                    FirstName = Guid.NewGuid().ToString(),
                    IsActive = true
                };
                repository.Insert(customer);
                repository.SaveChanges();

                repository.Delete(new Customer { Id = customer.Id });
                repository.SaveChanges();

                Assert.AreEqual(0, repository.Query().Count());
            }
        }

        [Test]
        public void DeleteUnAttached2()
        {
            using (IRepository<Customer> repository = CreateRepository<Customer>(CreateObjectContext()))
            {
                Customer customer = new Customer
                {
                    FirstName = Guid.NewGuid().ToString(),
                    IsActive = true
                };
                repository.Insert(customer);
                repository.SaveChanges();

                repository.Detach(customer);
                repository.Delete(customer);
                repository.SaveChanges();

                Assert.AreEqual(0, repository.Query().Count());
            }
        }

        protected override IRepository<TEntity> CreateRepository<TEntity>(ObjectContext objectContext)
        {
            return new SqlServerEntityFrameworkRepository<TEntity>(objectContext, null);
        }

        protected override ObjectContext CreateObjectContext()
        {
            ObjectContext objectContext = ((IObjectContextAdapter)new EfRepositoryDbEntities("name=EfRepositoryDbEntities")).ObjectContext;
            objectContext.ContextOptions.ProxyCreationEnabled = objectContext.ContextOptions.LazyLoadingEnabled = false;
            return objectContext;
        }
    }
}
