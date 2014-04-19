namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Domain;

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
