// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerCeEntityFrameworkRepository.cs" company="Labo">
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
//   Defines the SqlServerCeEntityFrameworkRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.SqlServerCe
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Data.SqlServerCe;
    using System.Linq.Expressions;

    using ErikEJ.SqlCe;

    using Labo.Common.Data.EntityFramework;
    using Labo.Common.Data.EntityFramework.Repository;
    using Labo.Common.Reflection;

    public sealed class SqlServerCeEntityFrameworkRepository<TEntity> : BaseEntityFrameworkRepository<TEntity>
        where TEntity : class
    {
        public SqlServerCeEntityFrameworkRepository(ObjectContext objectContext, IEntityFrameworkObjectContextManager objectContextManager)
            : base(objectContext, objectContextManager)
        {
        }

        public override void BulkInsert(string destinationTable, IEnumerable<TEntity> collection, IDbConnection connection, IDbTransaction dbTransaction = null)
        {
            SqlCeConnection sqlCeConnection;

            EntityConnection entityConnection = connection as EntityConnection;
            if (entityConnection != null)
            {
                sqlCeConnection = (SqlCeConnection)entityConnection.StoreConnection;
            }
            else
            {
                sqlCeConnection = (SqlCeConnection)connection;
            }

            SqlCeTransaction sqlCeTransaction = null;
            if (dbTransaction != null)
            {
                if (dbTransaction is EntityTransaction)
                {
                    sqlCeTransaction = (SqlCeTransaction)ReflectionHelper.GetPropertyValue(dbTransaction, "StoreTransaction");
                }
                else
                {
                    sqlCeTransaction = (SqlCeTransaction)dbTransaction;
                }
            }

            using (SqlCeBulkCopy sqlCeBulkCopy = sqlCeTransaction == null ? new SqlCeBulkCopy(sqlCeConnection) : new SqlCeBulkCopy(sqlCeConnection, SqlCeBulkCopyOptions.Default, sqlCeTransaction))
            {
                sqlCeBulkCopy.DestinationTableName = destinationTable;
                sqlCeBulkCopy.WriteToServer(collection);
            }
        }

        public override int DeleteAll()
        {
            throw new NotSupportedException("Batch delete is not supported for sql server ce");
        }

        public override int Delete(Expression<Func<TEntity, bool>> filterExpression)
        {
            throw new NotSupportedException("Batch delete is not supported for sql server ce");
        }

        public override int Update(Expression<Func<TEntity, TEntity>> updateExpression)
        {
            throw new NotSupportedException("Batch update is not supported for sql server ce");
        }

        public override int Update(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            throw new NotSupportedException("Batch update is not supported for sql server ce");
        }
    }
}
