namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

    public sealed class ProductMapping : EntityTypeConfiguration<Product>
    {
        public ProductMapping()
        {
            ToTable("Product");

            HasKey(x => x.Id);

            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
        }
    }
}