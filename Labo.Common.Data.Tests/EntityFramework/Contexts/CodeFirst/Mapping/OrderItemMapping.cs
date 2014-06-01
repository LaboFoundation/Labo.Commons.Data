namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    public sealed class OrderItemMapping : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMapping()
        {
            HasKey(t => new { t.OrderId, t.ProductId, t.Quantity });
            ToTable("OrderItem");
            Property(t => t.OrderId).HasColumnName("OrderId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(t => t.ProductId).HasColumnName("ProductId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(t => t.Quantity).HasColumnName("Quantity").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasRequired(t => t.Order).WithMany(t => t.OrderItems).HasForeignKey(d => d.OrderId);
            HasRequired(t => t.Product).WithMany(t => t.OrderItems).HasForeignKey(d => d.ProductId);
        }
    }
}