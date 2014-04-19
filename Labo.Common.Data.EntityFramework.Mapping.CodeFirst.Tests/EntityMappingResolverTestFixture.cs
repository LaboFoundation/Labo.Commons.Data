namespace Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Reflection;

    using Labo.Common.Data.EntityFramework.Mapping;
    using Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests.Data;
    using Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests.Data.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class EntityMappingResolverTestFixture
    {
        [Test]
        public void GetEntityMappings()
        {
            using (CodeFirstDbContext codeFirstEntities = new CodeFirstDbContext())
            {
                EntityMappingResolver entityMappingResolver = new EntityMappingResolver();
                IList<EntityMapping> entityMappings = entityMappingResolver.GetEntityMappings(((IObjectContextAdapter)codeFirstEntities).ObjectContext, Assembly.GetExecutingAssembly());
                
                Assert.AreEqual(1, entityMappings.Count);
                Assert.AreEqual(typeof(Customer), entityMappings[0].ClrType);
                Assert.AreEqual("[dbo].[Customer]", entityMappings[0].TableName);
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
