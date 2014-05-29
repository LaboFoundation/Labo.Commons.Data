// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the IRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
