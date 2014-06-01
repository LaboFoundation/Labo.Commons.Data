namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain
{
    using System;

    [Serializable]
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}