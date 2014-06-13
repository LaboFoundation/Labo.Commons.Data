// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerEntityFrameworkRepository.cs" company="Labo">
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
//   Defines the SqlServerEntityFrameworkRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.SqlServer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Data.SqlClient;

    using Labo.Common.Data.EntityFramework;
    using Labo.Common.Data.EntityFramework.Repository;
    using Labo.Common.Reflection;

    using Microsoft.Samples.EntityDataReader;

    public sealed class SqlServerEntityFrameworkRepository<TEntity> : BaseEntityFrameworkRepository<TEntity>
        where TEntity : class
    {
        public SqlServerEntityFrameworkRepository(ObjectContext objectContext, IEntityFrameworkObjectContextManager objectContextManager)
            : base(objectContext, objectContextManager)
        {
        }

        public override void BulkInsert(string destinationTable, IEnumerable<TEntity> collection, IDbConnection connection, System.Data.IDbTransaction dbTransaction = null)
        {
            SqlConnection sqlConnection;

            EntityConnection entityConnection = connection as EntityConnection;
            if (entityConnection != null)
            {
                sqlConnection = (SqlConnection)entityConnection.StoreConnection;
            }
            else
            {
                sqlConnection = (SqlConnection)connection;
            }

            SqlTransaction sqlTransaction = null;
            if (dbTransaction != null)
            {
                if (dbTransaction is EntityTransaction)
                {
                    sqlTransaction = (SqlTransaction)ReflectionHelper.GetPropertyValue(dbTransaction, "StoreTransaction");
                }
                else
                {
                    sqlTransaction = (SqlTransaction)dbTransaction;
                }
            }

            using (SqlBulkCopy sqlBulkCopy = sqlTransaction == null ? new SqlBulkCopy(sqlConnection) : new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                sqlBulkCopy.DestinationTableName = destinationTable;
                sqlBulkCopy.WriteToServer(collection.AsDataReader());
            }
        }
    }
}
