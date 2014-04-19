namespace Labo.Common.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;

    using Labo.Common.Data.Transaction;

    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query();

        void Insert(TEntity entity);

        void InsertOrUpdate(TEntity entity);

        void Update(TEntity entity);

        int Update(Expression<Func<TEntity, TEntity>> updateExpression);

        int Update(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);

        void UpdateProperties(TEntity entity, params Expression<Func<TEntity, object>>[] expression);

        void Delete(TEntity entity);

        int Delete();

        int Delete(Expression<Func<TEntity, bool>> filterExpression);

        IList<TEntity> LoadAll();

        //PagedResult<TEntity> LoadAll(int pageNo, int pageSize);

        //PagedResult<TEntity> PagedQuery(IQueryable<TEntity> objectQuery, int pageNo, int pageSize);

        void SaveChanges();

        void Attach(TEntity entity);

        void AttachTo(string entitySetName, TEntity entity);

        void DeleteUnAttached(TEntity entity);

        int ExecuteStoreCommand(string commandText, params object[] parameters);

        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        ITransaction BeginTransaction();

        void BulkInsert(string destinationTable, IEnumerable<TEntity> collection);

        void BulkInsert(IEnumerable<TEntity> collection);

        void BulkInsert(string destinationTable, IEnumerable<TEntity> collection, IDbConnection connection, IDbTransaction dbTransaction = null);

        void BulkInsert(IEnumerable<TEntity> collection, IDbConnection connection, IDbTransaction dbTransaction = null);

        IDbConnection GetConnection();

        ITransaction GetTransaction();
    }
}
