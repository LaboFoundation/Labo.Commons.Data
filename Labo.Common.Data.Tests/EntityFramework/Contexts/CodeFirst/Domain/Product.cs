namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
