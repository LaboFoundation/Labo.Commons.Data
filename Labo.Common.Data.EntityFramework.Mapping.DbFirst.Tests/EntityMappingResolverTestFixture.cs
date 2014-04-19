namespace Labo.Common.Data.EntityFramework.Mapping.DbFirst.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Labo.Common.Data.EntityFramework.Mapping;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.DbFirst;

    using NUnit.Framework;

    [TestFixture]
    public class EntityMappingResolverTestFixture
    {
        [Test]
        public void GetEntityMappings()
        {
            using (DbFirstEntities dbFirstEntities = new DbFirstEntities())
            {
                EntityMappingResolver entityMappingResolver = new EntityMappingResolver();
                IList<EntityMapping> entityMappings = entityMappingResolver.GetEntityMappings(dbFirstEntities, Assembly.GetExecutingAssembly());

                Assert.AreEqual(1, entityMappings.Count);
                Assert.AreEqual(typeof(Customer1), entityMappings[0].ClrType);
                Assert.AreEqual("[Customer]", entityMappings[0].TableName);
                Assert.AreEqual(2, entityMappings[0].PropertyMappings.Count);
                Assert.AreEqual("Id", entityMappings[0].PropertyMappings[0].ColumnName);
                Assert.AreEqual("Id", entityMappings[0].PropertyMappings[0].PropertyName);
                Assert.AreEqual("Name", entityMappings[0].PropertyMappings[1].ColumnName);
                Assert.AreEqual("Name", entityMappings[0].PropertyMappings[1].PropertyName);
                Assert.AreEqual(1, entityMappings[0].KeyMappings.Count);
                Assert.AreEqual("Id", entityMappings[0].KeyMappings[0].PropertyName);
                Assert.AreEqual("Id", entityMappings[0].KeyMappings[0].ColumnName);
            }
        }
    }
}
