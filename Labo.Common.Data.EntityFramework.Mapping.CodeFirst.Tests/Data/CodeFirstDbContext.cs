namespace Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests.Data
{
    using System.Data.Entity;

    using Labo.Common.Data.EntityFramework.Mapping.CodeFirst.Tests.Data.Mapping;

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
        }
    }
}
