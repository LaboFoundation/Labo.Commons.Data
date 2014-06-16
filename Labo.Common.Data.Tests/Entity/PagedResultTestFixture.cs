namespace Labo.Common.Data.Tests.Entity
{
    using System.Collections.Generic;

    using Labo.Common.Data.Entity;
    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class PagedResultTestFixture
    {
        [Test, Sequential]
        public void TotalPages(
            [Values(100, 15, 5, 0)]
            int totalItemsCount,
            [Values(20, 10, 10, 10)]
            int pageSize,
            [Values(5, 2, 1, 0)]
            int expected)
        {
            IPagedResult<Customer> pagedResult = new PagedResult<Customer>(new List<Customer>(), totalItemsCount, 1, pageSize);
            Assert.AreEqual(expected, pagedResult.TotalPages);
        }
    }
}
