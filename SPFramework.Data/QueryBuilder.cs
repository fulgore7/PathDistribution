using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SPFramework.Data
{
    public class QueryBuilder<T> where T : BaseQuery
    {
        protected internal T Query;

        /// <summary>
        /// Public constructor that sets the query object used (either StoredProcedure or Query).
        /// </summary>
        /// <param name="obj">The query object being stored</param>
        ///
        public QueryBuilder(T obj)
        {
            Query = obj;
        }

        /// <summary>
        /// This method is used to map the fields of an object to the fields in a DbDataReader in Fetches and FetchAlls
        /// </summary>
        /// <param name="objectRecordMapper">The Func delegate to execute</param>
        /// <returns>this</returns>
        ///
        public QueryBuilder<T> Map(Func<SqlDataReader, object> objectRecordMapper)
        {
            Query.Map(objectRecordMapper);
            return this;
        }

        public QueryBuilder<T> Map<TO>() where TO : class, new()
        {
            Query.Map<TO>();
            return this;
        }

        /// <summary>
        /// This method allows you to add a parameter specifying all necessary attributes
        /// </summary>
        /// <param name="parameterName">name of the sql parameter</param>
        /// <param name="value">value of the sql parameter</param>
        /// <param name="paramType">type of the DB parameter</param>
        /// <param name="paramSize">size of the DB parameter</param>
        /// <returns>QueryBuilder object</returns>
        ///
        public QueryBuilder<T> AddParameter(string parameterName, object value, SqlDbType paramType, int paramSize)
        {
            Query.AddParameter(parameterName, value, paramType, paramSize);
            return this;
        }

        /// <summary>
        /// This method allows you to add a parameter specifying all necessary attributes
        /// </summary>
        /// <param name="parameterName">name of the sql parameter</param>
        /// <param name="value">value of the sql parameter</param>
        /// <param name="paramType">type of the DB parameter</param>
        /// <returns>QueryBuilder object</returns>
        ///
        public QueryBuilder<T> AddParameter(string parameterName, object value, SqlDbType paramType)
        {
            Query.AddParameter(parameterName, value, paramType);
            return this;
        }

        /// <summary>
        /// Use this in an existing transaction. Ideally for use with the SimpleSql.CreateTransaction() method
        /// </summary>
        /// <param name="transaction">the existing transaction to use</param>
        /// <returns>this</returns>
        ///
        public QueryBuilder<T> WithTransaction(SqlTransaction transaction)
        {
            Query.WithTransaction(transaction);
            return this;
        }

        /// <summary>
        /// This will fetch a single record from the DB and map it to a concrete object
        /// </summary>
        /// <typeparam name="TO">The type of object to map this query to</typeparam>
        /// <returns>The object specified by the type parameter</returns>
        ///
        public TO Fetch<TO>() where TO : class, new()
        {
            return Query.Fetch<TO>();
        }

        /// <summary>
        /// This will map each record in the DB resultset to a concrete object
        /// </summary>
        /// <typeparam name="TO">The type of object to map this query to</typeparam>
        /// <returns>A list of strongly type objects specified by the type parameter</returns>
        ///
        public List<TO> FetchAll<TO>() where TO : class, new()
        {
            return Query.FetchAll<TO>();
        }

        /// <summary>
        /// Executes a query where you don't expect any results back
        /// </summary>
        ///
        public void Execute()
        {
            Query.Execute();
        }

        public void BulkInsert<TO>(IList<TO> list)
        {
            Query.BulkInsert(list);
        }

        /// <summary>
        /// Executes a query that returns a single value
        /// </summary>
        /// <typeparam name="TO">The type of value that you want returned</typeparam>
        /// <returns>The type specified by the type parameter</returns>
        ///
        public TO ExecuteScalar<TO>()
        {
            return Query.ExecuteScalar<TO>();
        }

        /// <summary>
        /// Fetches multiple resultSets
        /// </summary>
        /// <returns>A MultiResult object containing the collection of results</returns>
        ///
        public MultiResult FetchMultiple()
        {
            return Query.FetchMultiple();
        }
    }
}