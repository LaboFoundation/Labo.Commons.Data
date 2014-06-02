namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
