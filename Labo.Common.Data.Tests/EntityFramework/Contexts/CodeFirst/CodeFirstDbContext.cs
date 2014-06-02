namespace Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst
{
    using System.Data.Entity;

    using Labo.Common.Data.Tests.EntityFramework.Contexts.CodeFirst.Mapping;

    public class CodeFirstDbContext : DbContext
    {
        public CodeFirstDbContext()
            : this("CodeFirstDbContext")
        {
        }

        public CodeFirstDbContext(string connectionStringName)
            : base(connectionStringName)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new CustomerMapping());
            modelBuilder.Configurations.Add(new ProductMapping());
            modelBuilder.Configurations.Add(new OrderMapping());
            modelBuilder.Configurations.Add(new OrderItemMapping());
        }
    }
}
