namespace Labo.Common.Data.Session
{
    using System;

    public interface ISession : IDisposable
    {
        void Flush();
    }
}
