namespace Labo.Common.Data.SqlServer.Tests.Data
{
    using Labo.Common.Data.Entity;

    public partial class Customer : DirtyPropertyTrackingEntity<Customer>
    {
        public override Customer GetEntity()
        {
            return this;
        }
    }
}
