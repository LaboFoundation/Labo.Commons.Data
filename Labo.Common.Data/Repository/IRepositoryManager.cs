namespace Labo.Common.Data.Repository
{
    public interface IRepositoryManager
    {
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
    }
}
