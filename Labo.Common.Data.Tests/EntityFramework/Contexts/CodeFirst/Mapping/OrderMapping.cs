namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    public sealed class OrderMapping : EntityTypeConfiguration<Order>
    {
        public OrderMapping()
        {
            HasKey(t => t.Id);
            ToTable("Order");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CustomerId).HasColumnName("CustomerId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            HasRequired(t => t.Customer).WithMany(t => t.Orders).HasForeignKey(d => d.CustomerId);
        }
    }
}