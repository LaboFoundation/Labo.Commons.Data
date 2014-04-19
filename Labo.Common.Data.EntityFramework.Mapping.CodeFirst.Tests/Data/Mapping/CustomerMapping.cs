namespace Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests.Data.Domain;

    public sealed class CustomerMapping : EntityTypeConfiguration<Customer>
    {
        public CustomerMapping()
        {
            ToTable("Customer");

            HasKey(x => x.Id);

            Property(x => x.Name).HasMaxLength(100);
        }
    }
}
