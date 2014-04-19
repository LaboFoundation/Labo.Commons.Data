namespace Labo.Common.Data.Session
{
    /// <summary>
    /// The SessionFactory interface.
    /// </summary>
    public interface ISessionFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="ISession"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="ISession"/>.</returns>
        ISession CreateSession();
    }
}