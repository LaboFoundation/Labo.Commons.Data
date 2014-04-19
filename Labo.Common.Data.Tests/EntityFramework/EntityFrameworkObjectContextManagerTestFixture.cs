namespace Labo.Common.Data.Tests.EntityFramework
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;

    using Labo.Common.Data.EntityFramework;
    using Labo.Common.Data.EntityFramework.Exceptions;
    using Labo.Common.Data.EntityFramework.Mapping;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class EntityFrameworkObjectContextManagerTestFixture
    {
        private class Customer
        {
        }

        private class Order
        {
        }

        [Test]
        public void GetObjectContext()
        {
            DbContext dbContext1 = new DbContext("CodeFirstDbContext");
            DbContext dbContext2 = new DbContext("CodeFirstDbContext");

            ObjectContext objectContext1 = ((IObjectContextAdapter)dbContext1).ObjectContext;
            ObjectContext objectContext2 = ((IObjectContextAdapter)dbContext2).ObjectContext;

            IEntityMappingResolver entityMappingResolver = Substitute.For<IEntityMappingResolver>();
            
            List<EntityMapping> entityMappings1 = new List<EntityMapping> { new EntityMapping(typeof(Customer), null, null, null, null, "Customer") };
            List<EntityMapping> entityMappings2 = new List<EntityMapping> { new EntityMapping(typeof(Order), null, null, null, null, "Order") };

            entityMappingResolver.GetEntityMappings(objectContext1).Returns(entityMappings1);
            entityMappingResolver.GetEntityMappings(objectContext2).Returns(entityMappings2);

            IEntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(entityMappingResolver);
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext1);
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext2);

            Assert.AreEqual(objectContext1, entityFrameworkObjectContextManager.GetObjectContext<Customer>());
            Assert.AreEqual(objectContext2, entityFrameworkObjectContextManager.GetObjectContext<Order>());
        }

        [Test, ExpectedException(typeof(NoObjectContextRegisteredForTheSpecifiedEntityException))]
        public void GetObjectContextThrowsExceptionWhenNoObjectContextIsAssociatedWithSpecifiedEntity()
        {
            IEntityMappingResolver entityMappingResolver = Substitute.For<IEntityMappingResolver>();

            IEntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(entityMappingResolver);

            entityFrameworkObjectContextManager.GetObjectContext<Customer>();
        }

        [Test, ExpectedException(typeof(EntityAlreadyRegisteredWithAnotherObjectContextException))]
        public void RegisterObjectContextCreatorThrowsExceptionWhenTryingToRegisterAnEntityAlreadyAssociatedWithAnObjectContext()
        {
            DbContext dbContext1 = new DbContext("CodeFirstDbContext");
            DbContext dbContext2 = new DbContext("CodeFirstDbContext");

            ObjectContext objectContext1 = ((IObjectContextAdapter)dbContext1).ObjectContext;
            ObjectContext objectContext2 = ((IObjectContextAdapter)dbContext2).ObjectContext;

            IEntityMappingResolver entityMappingResolver = Substitute.For<IEntityMappingResolver>();

            List<EntityMapping> entityMappings1 = new List<EntityMapping> { new EntityMapping(typeof(Customer), null, null, null, null, "Customer") };

            entityMappingResolver.GetEntityMappings(objectContext1).Returns(entityMappings1);
            entityMappingResolver.GetEntityMappings(objectContext2).Returns(entityMappings1);

            IEntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(entityMappingResolver);
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext1);
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext2);
        }

        [Test]
        public void GetTableName()
        {
            DbContext dbContext = new DbContext("CodeFirstDbContext");

            ObjectContext objectContext1 = ((IObjectContextAdapter)dbContext).ObjectContext;

            IEntityMappingResolver entityMappingResolver = Substitute.For<IEntityMappingResolver>();

            List<EntityMapping> entityMappings = new List<EntityMapping> 
            { 
                new EntityMapping(typeof(Customer), null, null, null, null, "Customer"), 
                new EntityMapping(typeof(Order), null, null, null, null, "Order") 
            };

            entityMappingResolver.GetEntityMappings(objectContext1).Returns(entityMappings);

            IEntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(entityMappingResolver);
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext1);

            Assert.AreEqual("Customer", entityFrameworkObjectContextManager.GetTableName<Customer>());
            Assert.AreEqual("Order", entityFrameworkObjectContextManager.GetTableName<Order>());
        }

        [Test, ExpectedException(typeof(NoTableNameRegisteredForTheSpecifiedEntityException))]
        public void GetTableNameThrowsExceptionWhenNoTableNameIsAssociatedWithSpecifiedEntity()
        {
            DbContext dbContext = new DbContext("CodeFirstDbContext");

            ObjectContext objectContext1 = ((IObjectContextAdapter)dbContext).ObjectContext;

            IEntityMappingResolver entityMappingResolver = Substitute.For<IEntityMappingResolver>();

            List<EntityMapping> entityMappings = new List<EntityMapping> 
            { 
                new EntityMapping(typeof(Customer), null, null, null, null, "Customer") 
            };

            entityMappingResolver.GetEntityMappings(objectContext1).Returns(entityMappings);

            IEntityFrameworkObjectContextManager entityFrameworkObjectContextManager = new EntityFrameworkObjectContextManager(entityMappingResolver);
            entityFrameworkObjectContextManager.RegisterObjectContextCreator(() => objectContext1);

            entityFrameworkObjectContextManager.GetTableName<Order>();
        }
    }
}
