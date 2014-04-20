namespace Labo.Common.Data.EntityFramework
{
    using System;

    using Labo.Common.Data.Session;

    public sealed class EntityFrameworkSession : ISession
    {
        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
