namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}